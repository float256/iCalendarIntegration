using System;
using System.Collections.Generic;
using System.Linq;
using CalendarIntegrationCore.Models;
using CalendarIntegrationCore.Services;
using CalendarIntegrationCore.Services.DataProcessing;
using CalendarIntegrationCore.Services.DataSaving;
using CalendarIntegrationCore.Services.InitializationHandlers;
using CalendarIntegrationCore.Services.Repositories;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace CalendarIntegrationCore.Tests.Services.InitializationHandlers
{
    public class RoomAvailabilityInitializationHandlerTests
    {
        [Fact]
        public void RoomAvailabilityInitializationHandler_AddAvailabilityMessagesForRoomToQueue_RoomWithoutBookingInfo_OneMessageWithAllAvaliableDates()
        {
            // Arrange
            int synchronizationDaysInFuture = 200;
            Room room = new Room
            {
                Id = 2,
                HotelId = 1234,
                Name = "room",
                TLApiCode = "1234",
                Url = "https://example.com"
            };
            List<BookingInfo> bookingInfoRepositoryData = new List<BookingInfo>
            {
                new BookingInfo
                {
                    Id = 0,
                    StartBooking = DateTime.Now,
                    EndBooking = DateTime.Now.AddDays(12),
                    RoomId = 1
                },
                new BookingInfo
                {
                    Id = 0,
                    StartBooking = DateTime.Now.AddDays(15),
                    EndBooking = DateTime.Now.AddDays(18),
                    RoomId = 1
                }
            };
            List<AvailabilityStatusMessage> expectedAvailabilityStatusMessageQueue = new List<AvailabilityStatusMessage>
            {
                new AvailabilityStatusMessage
                {
                    RoomId = room.Id,
                    State = BookingLimitType.Available,
                    StartDate = DateTime.Today,
                    EndDate = DateTime.Today.AddDays(synchronizationDaysInFuture)
                }
            };
            List<AvailabilityStatusMessage> actualAvailabilityStatusMessageQueue = new List<AvailabilityStatusMessage>();

            IOptions<DateSynchronizationCommonOptions> dateSyncOptions =
                Options.Create(new DateSynchronizationCommonOptions
                {
                    SynchronizationDaysInFuture = synchronizationDaysInFuture
                });
            
            ITodayBoundary todayBoundary = new TodayBoundary(dateSyncOptions);
            IAvailabilityMessageConverter availabilityMessageConverter = new AvailabilityMessageConverter();;

            Mock<IBookingInfoRepository> mockBookingInfoRepository = new Mock<IBookingInfoRepository>(MockBehavior.Strict);;
            Mock<IAvailabilityStatusMessageQueue> mockAvailabilityStatusMessageQueue = new Mock<IAvailabilityStatusMessageQueue>(MockBehavior.Strict);;
            
            mockBookingInfoRepository.Setup(repository => repository.GetByRoomId(It.IsAny<int>()))
                .Returns((int roomId) =>
                {
                    return bookingInfoRepositoryData.Where(bookingInfo => bookingInfo.RoomId == roomId)
                        .OrderBy(bookingInfo => bookingInfo.StartBooking)
                        .ToList();                
                });
            mockAvailabilityStatusMessageQueue.Setup(repository =>
                repository.EnqueueMultiple(It.IsAny<List<AvailabilityStatusMessage>>()))
                    .Callback((List<AvailabilityStatusMessage> availStatusMessage) =>
                    {
                        actualAvailabilityStatusMessageQueue.AddRange(availStatusMessage);
                    });

            // Act
            RoomAvailabilityInitializationHandler roomAvailInitHandler = new RoomAvailabilityInitializationHandler(
                mockBookingInfoRepository.Object,
                todayBoundary,
                mockAvailabilityStatusMessageQueue.Object,
                availabilityMessageConverter);
            roomAvailInitHandler.AddAvailabilityMessagesForRoomToQueue(room);
            
            // Assert
            Assert.Equal(expectedAvailabilityStatusMessageQueue.Count, actualAvailabilityStatusMessageQueue.Count);
            for (int i = 0; i < expectedAvailabilityStatusMessageQueue.Count; i++)
            {
                AvailabilityStatusMessage actualMessage = actualAvailabilityStatusMessageQueue[i];
                AvailabilityStatusMessage expectedMessage = expectedAvailabilityStatusMessageQueue[i];
                
                Assert.Equal(expectedMessage.State, actualMessage.State);
                Assert.Equal(expectedMessage.StartDate, actualMessage.StartDate);
                Assert.Equal(expectedMessage.EndDate, actualMessage.EndDate);
                Assert.Equal(expectedMessage.RoomId, expectedMessage.RoomId);
            }
        }

        [Fact]
        public void RoomAvailabilityInitializationHandler_AddAvailabilityMessagesForRoomToQueue_RoomWithTwoBookingInfos_CorrectMessages()
        {
            // Arrange
            int synchronizationDaysInFuture = 200;
            Room room = new Room
            {
                Id = 1,
                HotelId = 1234,
                Name = "room",
                TLApiCode = "1234",
                Url = "https://example.com"
            };
            List<BookingInfo> bookingInfoRepositoryData = new List<BookingInfo>
            {
                new BookingInfo
                {
                    Id = 0,
                    StartBooking = DateTime.Today.AddDays(3),
                    EndBooking = DateTime.Today.AddDays(12),
                    RoomId = 1
                },
                new BookingInfo
                {
                    Id = 0,
                    StartBooking = DateTime.Today.AddDays(15),
                    EndBooking = DateTime.Today.AddDays(18),
                    RoomId = 1
                }
            };
            List<AvailabilityStatusMessage> expectedAvailabilityStatusMessageQueue = new List<AvailabilityStatusMessage>
            {
                new AvailabilityStatusMessage
                {
                    RoomId = room.Id,
                    State = BookingLimitType.Available,
                    StartDate = DateTime.Today,
                    EndDate = DateTime.Today.AddDays(2)
                },
                new AvailabilityStatusMessage
                {
                    RoomId = room.Id,
                    State = BookingLimitType.Occupied,
                    StartDate = DateTime.Today.AddDays(3),
                    EndDate = DateTime.Today.AddDays(11)
                },
                new AvailabilityStatusMessage
                {
                    RoomId = room.Id,
                    State = BookingLimitType.Available,
                    StartDate = DateTime.Today.AddDays(12),
                    EndDate = DateTime.Today.AddDays(14)
                },
                new AvailabilityStatusMessage
                {
                    RoomId = room.Id,
                    State = BookingLimitType.Occupied,
                    StartDate = DateTime.Today.AddDays(15),
                    EndDate = DateTime.Today.AddDays(17)
                },
                new AvailabilityStatusMessage
                {
                    RoomId = room.Id,
                    State = BookingLimitType.Available,
                    StartDate = DateTime.Today.AddDays(18),
                    EndDate = DateTime.Today.AddDays(synchronizationDaysInFuture)
                }
            };
            List<AvailabilityStatusMessage> actualAvailabilityStatusMessageQueue = new List<AvailabilityStatusMessage>();

            IOptions<DateSynchronizationCommonOptions> dateSyncOptions =
                Options.Create(new DateSynchronizationCommonOptions
                {
                    SynchronizationDaysInFuture = synchronizationDaysInFuture
                });
            
            ITodayBoundary todayBoundary = new TodayBoundary(dateSyncOptions);
            IAvailabilityMessageConverter availabilityMessageConverter = new AvailabilityMessageConverter();;

            Mock<IBookingInfoRepository> mockBookingInfoRepository = new Mock<IBookingInfoRepository>(MockBehavior.Strict);;
            Mock<IAvailabilityStatusMessageQueue> mockAvailabilityStatusMessageQueue = new Mock<IAvailabilityStatusMessageQueue>(MockBehavior.Strict);;
            
            mockBookingInfoRepository.Setup(repository => repository.GetByRoomId(It.IsAny<int>()))
                .Returns((int roomId) =>
                {
                    return bookingInfoRepositoryData.Where(bookingInfo => bookingInfo.RoomId == roomId)
                        .OrderBy(bookingInfo => bookingInfo.StartBooking)
                        .ToList();                
                });
            mockAvailabilityStatusMessageQueue.Setup(repository =>
                repository.EnqueueMultiple(It.IsAny<List<AvailabilityStatusMessage>>()))
                    .Callback((List<AvailabilityStatusMessage> availStatusMessage) =>
                    {
                        actualAvailabilityStatusMessageQueue.AddRange(availStatusMessage);
                    });

            // Act
            RoomAvailabilityInitializationHandler roomAvailInitHandler = new RoomAvailabilityInitializationHandler(
                mockBookingInfoRepository.Object,
                todayBoundary,
                mockAvailabilityStatusMessageQueue.Object,
                availabilityMessageConverter);
            roomAvailInitHandler.AddAvailabilityMessagesForRoomToQueue(room);
            
            // Assert
            Assert.Equal(expectedAvailabilityStatusMessageQueue.Count, actualAvailabilityStatusMessageQueue.Count);
            for (int i = 0; i < expectedAvailabilityStatusMessageQueue.Count; i++)
            {
                AvailabilityStatusMessage actualMessage = actualAvailabilityStatusMessageQueue[i];
                AvailabilityStatusMessage expectedMessage = expectedAvailabilityStatusMessageQueue[i];
                
                Assert.Equal(expectedMessage.State, actualMessage.State);
                Assert.Equal(expectedMessage.StartDate, actualMessage.StartDate);
                Assert.Equal(expectedMessage.EndDate, actualMessage.EndDate);
                Assert.Equal(expectedMessage.RoomId, expectedMessage.RoomId);
            }   
        }
    }
}