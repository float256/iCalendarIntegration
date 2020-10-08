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
                    StartDate = DateTime.UtcNow.Date.AddDays(-1),
                    EndDate = DateTime.UtcNow.Date.AddDays(synchronizationDaysInFuture)
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
                    StartBooking = DateTime.UtcNow.Date.AddDays(3),
                    EndBooking = DateTime.UtcNow.Date.AddDays(12),
                    RoomId = 1
                },
                new BookingInfo
                {
                    Id = 0,
                    StartBooking = DateTime.UtcNow.Date.AddDays(15),
                    EndBooking = DateTime.UtcNow.Date.AddDays(18),
                    RoomId = 1
                }
            };
            List<AvailabilityStatusMessage> expectedAvailabilityStatusMessageQueue = new List<AvailabilityStatusMessage>
            {
                new AvailabilityStatusMessage
                {
                    RoomId = room.Id,
                    State = BookingLimitType.Available,
                    StartDate = DateTime.UtcNow.Date.AddDays(-1),
                    EndDate = DateTime.UtcNow.Date.AddDays(2)
                },
                new AvailabilityStatusMessage
                {
                    RoomId = room.Id,
                    State = BookingLimitType.Occupied,
                    StartDate = DateTime.UtcNow.Date.AddDays(3),
                    EndDate = DateTime.UtcNow.Date.AddDays(11)
                },
                new AvailabilityStatusMessage
                {
                    RoomId = room.Id,
                    State = BookingLimitType.Available,
                    StartDate = DateTime.UtcNow.Date.AddDays(12),
                    EndDate = DateTime.UtcNow.Date.AddDays(14)
                },
                new AvailabilityStatusMessage
                {
                    RoomId = room.Id,
                    State = BookingLimitType.Occupied,
                    StartDate = DateTime.UtcNow.Date.AddDays(15),
                    EndDate = DateTime.UtcNow.Date.AddDays(17)
                },
                new AvailabilityStatusMessage
                {
                    RoomId = room.Id,
                    State = BookingLimitType.Available,
                    StartDate = DateTime.UtcNow.Date.AddDays(18),
                    EndDate = DateTime.UtcNow.Date.AddDays(synchronizationDaysInFuture)
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

            Mock<IBookingInfoRepository> mockBookingInfoRepository = new Mock<IBookingInfoRepository>(MockBehavior.Strict);
            Mock<IAvailabilityStatusMessageQueue> mockAvailabilityStatusMessageQueue = new Mock<IAvailabilityStatusMessageQueue>(MockBehavior.Strict);;
            
            mockBookingInfoRepository.Setup(repository => repository.GetByRoomId(room.Id))
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
        public void RoomAvailabilityInitializationHandler_AddAvailabilityMessagesForRoomToQueue_RoomWithIntersectedBookingInfos_Exception()
        {
            // Arrange
            int synchronizationDaysInFuture = 200;
            string expectedExceptionMessage = "Intersection of dates is not allowed";
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
                    StartBooking = DateTime.UtcNow.Date.AddDays(3),
                    EndBooking = DateTime.UtcNow.Date.AddDays(12),
                    RoomId = 1
                },
                new BookingInfo
                {
                    Id = 0,
                    StartBooking = DateTime.UtcNow.Date.AddDays(15),
                    EndBooking = DateTime.UtcNow.Date.AddDays(18),
                    RoomId = 1
                },
                new BookingInfo
                {
                    Id = 0,
                    StartBooking = DateTime.UtcNow.Date.AddDays(17),
                    EndBooking = DateTime.UtcNow.Date.AddDays(21),
                    RoomId = 1
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

            Mock<IBookingInfoRepository> mockBookingInfoRepository = new Mock<IBookingInfoRepository>(MockBehavior.Strict);
            Mock<IAvailabilityStatusMessageQueue> mockAvailabilityStatusMessageQueue = new Mock<IAvailabilityStatusMessageQueue>(MockBehavior.Strict);;
            
            mockBookingInfoRepository.Setup(repository => repository.GetByRoomId(room.Id))
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
            Action act = () => roomAvailInitHandler.AddAvailabilityMessagesForRoomToQueue(room);
            
            //Assert
            RoomAvailabilityInitializationHandlerException  exception = Assert.Throws<RoomAvailabilityInitializationHandlerException>(act);
            Assert.Equal(expectedExceptionMessage, exception.Message);
        }
    }
}