using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using CalendarIntegrationCore.Models;
using CalendarIntegrationCore.Services;
using CalendarIntegrationCore.Services.Repositories;
using Xunit;
using Moq;
using Moq.Protected;
using Microsoft.Extensions.Logging;

namespace CalendarIntegrationCore.Tests
{
    public class AvailabilityInfoSenderTests
    {
        [Fact]
        public void AvailabilityInfoSender_GetCalendarByUrl_ResponseIsOK_StringWithCalendar()
        {
            // Arrange
            string expectedCalendar = String.Join(Environment.NewLine,
                "BEGIN:VCALENDAR",
                "VERSION:2.0",
                "CALSCALE:GREGORIAN",
                "METHOD:PUBLISH",
                "END:VCALENDAR");
            string url = "http://example.com";
            
            Mock<IHotelRepository> mockHotelRepository = new Mock<IHotelRepository>(MockBehavior.Strict);
            Mock<IRoomRepository> mockRoomRepository = new Mock<IRoomRepository>(MockBehavior.Strict);
            Mock<IBookingInfoRepository> mockBookingInfoRepository = new Mock<IBookingInfoRepository>(MockBehavior.Strict);
            Mock<ICalendarParser> mockCalendarParser = new Mock<ICalendarParser>(MockBehavior.Strict);
            Mock<IHttpClientFactory> mockHttpClientFactory = new Mock<IHttpClientFactory>(MockBehavior.Strict);  
            Mock<HttpMessageHandler> mockHttpMessageHandler = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            Mock<ILogger<AvailabilityInfoSender>> mockLogger = new Mock<ILogger<AvailabilityInfoSender>>(MockBehavior.Strict);

            mockHttpMessageHandler.Protected()  
                .Setup<Task<HttpResponseMessage>>("SendAsync", 
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())  
                .ReturnsAsync(new HttpResponseMessage  
                {  
                    StatusCode = HttpStatusCode.OK,  
                    Content = new StringContent(expectedCalendar)  
                });

            HttpClient httpClient = new HttpClient(mockHttpMessageHandler.Object)
            {
                BaseAddress = new Uri(url)
            };
            mockHttpClientFactory.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(httpClient);
            Mock<IAvailabilityInfoSender> mockAvailabilityInfoSender = new Mock<IAvailabilityInfoSender>(MockBehavior.Strict);
            
            // Act
            var availabilityInfoSender = new AvailabilityInfoSender(
                mockHotelRepository.Object, 
                mockRoomRepository.Object,
                mockBookingInfoRepository.Object,
                mockCalendarParser.Object, 
                mockHttpClientFactory.Object,
                mockLogger.Object);
            string actualCalendar = availabilityInfoSender.GetCalendarByUrl(url);
            
            // Assert
            Assert.Equal(expectedCalendar, actualCalendar);
        }
        
        [Fact]
        public void AvailabilityInfoSender_GetCalendarByUrl_ResponseIsNotOK_StringWithCalendar()
        {
            // Arrange
            string expectedCalendar = String.Empty;
            string url = "http://example.com";

            Mock<IHotelRepository> mockHotelRepository = new Mock<IHotelRepository>(MockBehavior.Strict);
            Mock<IRoomRepository> mockRoomRepository = new Mock<IRoomRepository>(MockBehavior.Strict);
            Mock<IBookingInfoRepository> mockBookingInfoRepository = new Mock<IBookingInfoRepository>(MockBehavior.Strict);
            Mock<ICalendarParser> mockCalendarParser = new Mock<ICalendarParser>(MockBehavior.Strict);
            Mock<IHttpClientFactory> mockHttpClientFactory = new Mock<IHttpClientFactory>(MockBehavior.Strict);  
            Mock<HttpMessageHandler> mockHttpMessageHandler = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            Mock<ILogger<AvailabilityInfoSender>> mockLogger = new Mock<ILogger<AvailabilityInfoSender>>(MockBehavior.Strict);

            mockHttpMessageHandler.Protected()  
                .Setup<Task<HttpResponseMessage>>("SendAsync", 
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())  
                .ReturnsAsync(new HttpResponseMessage  
                {  
                    StatusCode = HttpStatusCode.NotFound,  
                    Content = new StringContent(expectedCalendar)  
                });

            HttpClient httpClient = new HttpClient(mockHttpMessageHandler.Object)
            {
                BaseAddress = new Uri(url)
            };
            mockHttpClientFactory.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(httpClient);
            Mock<IAvailabilityInfoSender> mockAvailabilityInfoSender = new Mock<IAvailabilityInfoSender>(MockBehavior.Strict);
            
            // Act
            var availabilityInfoSender = new AvailabilityInfoSender(
                mockHotelRepository.Object, 
                mockRoomRepository.Object,
                mockBookingInfoRepository.Object,
                mockCalendarParser.Object, mockHttpClientFactory.Object, 
                mockLogger.Object);
            string actualCalendar = availabilityInfoSender.GetCalendarByUrl(url);
            
            // Assert
            Assert.Equal(expectedCalendar, actualCalendar);
        }

        [Fact]
        public void AvailabilityInfoSender_GetChanges_CalendarWithPartOfPreviousAndNewDates_CorrectBookingInfoChanges()
        {
            // Arrange
            int roomId = 1;
            BookingInfoChanges expected = new BookingInfoChanges
            {
                AddedBookingInfo = new List<BookingInfo>
                {
                    new BookingInfo
                    {
                        RoomId = roomId, 
                        StartBooking = new DateTime(1953, 2, 12),
                        EndBooking = new DateTime(1953, 6, 3),
                    },
                    new BookingInfo
                    { 
                        RoomId = roomId, 
                        StartBooking = new DateTime(1984, 12, 30),
                        EndBooking = new DateTime(1984, 12, 31),
                    }
                },
                RemovedBookingInfo = new List<BookingInfo>
                {
                    new BookingInfo
                    {
                        Id = 1,
                        RoomId = roomId,
                        StartBooking = new DateTime(2020, 3, 14),
                        EndBooking = new DateTime(2020, 3, 17),
                    },
                    new BookingInfo
                    {
                        Id = 3, 
                        RoomId = roomId, 
                        StartBooking = new DateTime(2021, 5, 14),
                        EndBooking = new DateTime(2021, 5, 17),                        
                    }
                }
            };
            List<BookingInfo> bookingInfoRepositoryData = new List<BookingInfo>
            {
                new BookingInfo
                {
                    Id = 1, 
                    RoomId = roomId, 
                    StartBooking = new DateTime(2020, 3, 14),
                    EndBooking = new DateTime(2020, 3, 17),
                },
                new BookingInfo
                {
                    Id = 2, 
                    RoomId = roomId, 
                    StartBooking = new DateTime(2019, 4, 16),
                    EndBooking = new DateTime(2019, 4, 21),
                },
                new BookingInfo
                {
                    Id = 3, 
                    RoomId = roomId, 
                    StartBooking = new DateTime(2021, 5, 14),
                    EndBooking = new DateTime(2021, 5, 17),
                },
            };
            List<BookingInfo> parsedCalendarBookingInfo = new List<BookingInfo>
            {
                new BookingInfo
                {
                    RoomId = roomId,
                    StartBooking = new DateTime(2019, 4, 16),
                    EndBooking = new DateTime(2019, 4, 21),
                },
                new BookingInfo
                {
                    RoomId = roomId, 
                    StartBooking = new DateTime(1984, 12, 30),
                    EndBooking = new DateTime(1984, 12, 31),
                },
                new BookingInfo
                {
                    RoomId = roomId, 
                    StartBooking = new DateTime(1953, 2, 12),
                    EndBooking = new DateTime(1953, 6, 3),
                }
            };
            Mock<IHotelRepository> mockHotelRepository = new Mock<IHotelRepository>(MockBehavior.Strict);
            Mock<IRoomRepository> mockRoomRepository = new Mock<IRoomRepository>(MockBehavior.Strict);
            Mock<ICalendarParser> mockCalendarParser = new Mock<ICalendarParser>(MockBehavior.Strict);
            Mock<IHttpClientFactory> mockHttpClientFactory = new Mock<IHttpClientFactory>(MockBehavior.Strict);  
            Mock<IBookingInfoRepository> mockBookingInfoRepository = new Mock<IBookingInfoRepository>(MockBehavior.Strict);
            Mock<ILogger<AvailabilityInfoSender>> mockLogger = new Mock<ILogger<AvailabilityInfoSender>>(MockBehavior.Strict);

            mockBookingInfoRepository.Setup(bookingInfo => bookingInfo.GetByRoomId(It.IsAny<int>()))
                .Returns<int>(roomId =>
                    bookingInfoRepositoryData.FindAll(elem => (elem.RoomId == roomId))
                        .OrderBy(elem => elem.StartBooking).ToList());
            mockCalendarParser.Setup(parser => parser.ParseCalendar(It.IsAny<string>(), It.IsAny<int>()))
                .Returns<string, int>((calendar, roomId) =>
                    parsedCalendarBookingInfo.OrderBy(elem => elem.StartBooking).ToList());
            
            // Act
            AvailabilityInfoSender infoSender = new AvailabilityInfoSender(
                mockHotelRepository.Object, 
                mockRoomRepository.Object,
                mockBookingInfoRepository.Object, 
                mockCalendarParser.Object,
                mockHttpClientFactory.Object,
                mockLogger.Object);
            BookingInfoChanges actual = infoSender.GetChanges(String.Empty, roomId);
            
            // Assert
            Assert.Equal(expected.AddedBookingInfo.Count, actual.AddedBookingInfo.Count);
            Assert.Equal(expected.RemovedBookingInfo.Count, actual.RemovedBookingInfo.Count);
            for (int i = 0; i < expected.AddedBookingInfo.Count; i++)
            {
                var currExpectedBookingInfo = expected.AddedBookingInfo[i];
                var currActualBookingInfo = actual.AddedBookingInfo[i];
                Assert.Equal(currExpectedBookingInfo.StartBooking, currActualBookingInfo.StartBooking);
                Assert.Equal(currExpectedBookingInfo.EndBooking, currActualBookingInfo.EndBooking);
                Assert.Equal(currExpectedBookingInfo.RoomId, currActualBookingInfo.RoomId);
            }
            for (int i = 0; i < expected.RemovedBookingInfo.Count; i++)
            {
                var currExpectedBookingInfo = expected.RemovedBookingInfo[i];
                var currActualBookingInfo = actual.RemovedBookingInfo[i];
                Assert.Equal(currExpectedBookingInfo.Id, currActualBookingInfo.Id);
                Assert.Equal(currExpectedBookingInfo.StartBooking, currActualBookingInfo.StartBooking);
                Assert.Equal(currExpectedBookingInfo.EndBooking, currActualBookingInfo.EndBooking);
                Assert.Equal(currExpectedBookingInfo.RoomId, currActualBookingInfo.RoomId);
            }
        }

        [Fact]
        public void AvailabilityInfoSender_GetChanges_CalendarWithoutDatesAndWithoutPreviousDates_EmptyBookingInfoChangesObject()
        {
            // Arrange
            List<BookingInfo> parsedCalendarBookingInfo = new List<BookingInfo>{};
            List<BookingInfo> bookingInfoRepositoryData = new List<BookingInfo>();
            BookingInfoChanges expected = new BookingInfoChanges();
            int roomId = 1;
            
;           Mock<IHotelRepository> mockHotelRepository = new Mock<IHotelRepository>(MockBehavior.Strict);
            Mock<IRoomRepository> mockRoomRepository = new Mock<IRoomRepository>(MockBehavior.Strict);
            Mock<IBookingInfoRepository> mockBookingInfoRepository = new Mock<IBookingInfoRepository>(MockBehavior.Strict);
            Mock<ICalendarParser> mockCalendarParser = new Mock<ICalendarParser>(MockBehavior.Strict);
            Mock<ILogger<AvailabilityInfoSender>> mockLogger = new Mock<ILogger<AvailabilityInfoSender>>(MockBehavior.Strict);
            Mock<IHttpClientFactory> mockHttpClientFactory = new Mock<IHttpClientFactory>(MockBehavior.Strict); 
            mockBookingInfoRepository.Setup(bookingInfo => bookingInfo.GetByRoomId(It.IsAny<int>()))
                .Returns<int>(roomId =>
                    bookingInfoRepositoryData.FindAll(elem => (elem.RoomId == roomId))
                        .OrderBy(elem => elem.StartBooking).ToList());
            mockCalendarParser.Setup(parser => parser.ParseCalendar(It.IsAny<string>(), It.IsAny<int>()))
                .Returns<string, int>((calendar, roomId) =>
                    parsedCalendarBookingInfo.OrderBy(elem => elem.StartBooking).ToList());
            
            // Act
            AvailabilityInfoSender infoSender = new AvailabilityInfoSender(
                mockHotelRepository.Object,
                mockRoomRepository.Object,
                mockBookingInfoRepository.Object, 
                mockCalendarParser.Object, 
                mockHttpClientFactory.Object,
                mockLogger.Object);
            BookingInfoChanges actual = infoSender.GetChanges(String.Empty, roomId);
            
            // Assert
            Assert.Equal(expected.AddedBookingInfo.Count, actual.AddedBookingInfo.Count);
            Assert.Equal(expected.RemovedBookingInfo.Count, actual.RemovedBookingInfo.Count);
        }

        [Fact]
        public void AvailabilityInfoSender_SaveChanges_NotEmptyBookingInfoChangesObject_ChangesHaveBeenWrittenToDatabase()
        {
            // Arrange
            int roomId = 1;
            List<BookingInfo> expectedBookingInfoRepositoryData = new List<BookingInfo>
            {
                new BookingInfo
                {
                    Id = 1, 
                    RoomId = roomId, 
                    StartBooking = new DateTime(2012, 3, 24),
                    EndBooking = new DateTime(2012, 3, 27),
                },
                new BookingInfo
                {
                    Id = 4,
                    RoomId = roomId, 
                    StartBooking = new DateTime(1953, 2, 12),
                    EndBooking = new DateTime(1953, 6, 3),
                },
                new BookingInfo
                { 
                    Id = 5,
                    RoomId = roomId, 
                    StartBooking = new DateTime(1984, 12, 30),
                    EndBooking = new DateTime(1984, 12, 31),
                }
            };
            List<BookingInfo> actualBookingInfoRepositoryData = new List<BookingInfo>
            {
                new BookingInfo
                {
                    Id = 1, 
                    RoomId = roomId, 
                    StartBooking = new DateTime(2012, 3, 24),
                    EndBooking = new DateTime(2012, 3, 27),
                },
                new BookingInfo
                {
                    Id = 2, 
                    RoomId = roomId, 
                    StartBooking = new DateTime(2019, 4, 16),
                    EndBooking = new DateTime(2019, 4, 21),
                },
                new BookingInfo
                {
                    Id = 3, 
                    RoomId = roomId, 
                    StartBooking = new DateTime(2021, 5, 14),
                    EndBooking = new DateTime(2021, 5, 17),
                },
            };
            int maxBookingInfoId = 3;
            BookingInfoChanges changes = new BookingInfoChanges
            {
                AddedBookingInfo = new List<BookingInfo>
                {
                    new BookingInfo
                    {
                        RoomId = roomId, 
                        StartBooking = new DateTime(1953, 2, 12),
                        EndBooking = new DateTime(1953, 6, 3),
                    },
                    new BookingInfo
                    { 
                        RoomId = roomId, 
                        StartBooking = new DateTime(1984, 12, 30),
                        EndBooking = new DateTime(1984, 12, 31),
                    }
                },
                RemovedBookingInfo = new List<BookingInfo>
                {
                    new BookingInfo
                    {
                        Id = 2,
                        RoomId = roomId,
                        StartBooking = new DateTime(2019, 4, 16),
                        EndBooking = new DateTime(2019, 4, 21),
                    },
                    new BookingInfo
                    {
                        Id = 3, 
                        RoomId = roomId, 
                        StartBooking = new DateTime(2021, 5, 14),
                        EndBooking = new DateTime(2021, 5, 17),                        
                    }
                }
            };
            
            Mock<IHotelRepository> mockHotelRepository = new Mock<IHotelRepository>(MockBehavior.Strict);
            Mock<IRoomRepository> mockRoomRepository = new Mock<IRoomRepository>(MockBehavior.Strict);
            Mock<ICalendarParser> mockCalendarParser = new Mock<ICalendarParser>(MockBehavior.Strict);
            Mock<IHttpClientFactory> mockHttpClientFactory = new Mock<IHttpClientFactory>(MockBehavior.Strict);  
            Mock<IBookingInfoRepository> mockBookingInfoRepository = new Mock<IBookingInfoRepository>(MockBehavior.Strict);
            Mock<ILogger<AvailabilityInfoSender>> mockLogger = new Mock<ILogger<AvailabilityInfoSender>>(MockBehavior.Strict);

            mockBookingInfoRepository.Setup(repository => repository.Add(It.IsAny<BookingInfo>()))
                .Callback((BookingInfo bookingInfo) =>
                {
                    maxBookingInfoId++;
                    bookingInfo.Id = maxBookingInfoId;
                    actualBookingInfoRepositoryData.Add(bookingInfo);
                });
            mockBookingInfoRepository.Setup(repository => repository.Delete(It.IsAny<int>()))
                .Callback((int id) =>
                {
                    actualBookingInfoRepositoryData.RemoveAll(elem => elem.Id == id);
                });
            
            // Act
            AvailabilityInfoSender infoSender = new AvailabilityInfoSender(
                mockHotelRepository.Object, 
                mockRoomRepository.Object,
                mockBookingInfoRepository.Object, 
                mockCalendarParser.Object, 
                mockHttpClientFactory.Object, 
                mockLogger.Object);
            infoSender.SaveChanges(changes);
            
            // Assert
            Assert.Equal(expectedBookingInfoRepositoryData.Count, actualBookingInfoRepositoryData.Count);
            for (int i = 0; i < expectedBookingInfoRepositoryData.Count; i++)
            {
                BookingInfo currExpectedBookingInfo = expectedBookingInfoRepositoryData[i];
                BookingInfo currActualBookingInfo = actualBookingInfoRepositoryData[i];
                
                Assert.Equal(currExpectedBookingInfo.Id, currActualBookingInfo.Id);
                Assert.Equal(currExpectedBookingInfo.StartBooking, currActualBookingInfo.StartBooking);
                Assert.Equal(currExpectedBookingInfo.EndBooking, currActualBookingInfo.EndBooking);
                Assert.Equal(currExpectedBookingInfo.RoomId, currActualBookingInfo.RoomId);
            }
        }
        
        [Fact]
        public void AvailabilityInfoSender_SaveChanges_EmptyBookingInfoChangesObject_NoChanges()
        {
            // Arrange
            int roomId = 1;
            List<BookingInfo> expectedBookingInfoRepositoryData = new List<BookingInfo>
            {
                new BookingInfo
                {
                    Id = 1, 
                    RoomId = roomId, 
                    StartBooking = new DateTime(2012, 3, 24),
                    EndBooking = new DateTime(2012, 3, 27),
                },
                new BookingInfo
                {
                    Id = 2, 
                    RoomId = roomId, 
                    StartBooking = new DateTime(2019, 4, 16),
                    EndBooking = new DateTime(2019, 4, 21),
                },
            };
            List<BookingInfo> actualBookingInfoRepositoryData = new List<BookingInfo>
            {
                new BookingInfo
                {
                    Id = 1, 
                    RoomId = roomId, 
                    StartBooking = new DateTime(2012, 3, 24),
                    EndBooking = new DateTime(2012, 3, 27),
                },
                new BookingInfo
                {
                    Id = 2, 
                    RoomId = roomId, 
                    StartBooking = new DateTime(2019, 4, 16),
                    EndBooking = new DateTime(2019, 4, 21),
                },
            };
            int maxBookingInfoId = 2;
            BookingInfoChanges changes = new BookingInfoChanges();
            
            Mock<IHotelRepository> mockHotelRepository = new Mock<IHotelRepository>(MockBehavior.Strict);
            Mock<IRoomRepository> mockRoomRepository = new Mock<IRoomRepository>(MockBehavior.Strict);
            Mock<ICalendarParser> mockCalendarParser = new Mock<ICalendarParser>(MockBehavior.Strict);
            Mock<IHttpClientFactory> mockHttpClientFactory = new Mock<IHttpClientFactory>(MockBehavior.Strict);  
            Mock<IBookingInfoRepository> mockBookingInfoRepository = new Mock<IBookingInfoRepository>(MockBehavior.Strict);
            Mock<ILogger<AvailabilityInfoSender>> mockLogger = new Mock<ILogger<AvailabilityInfoSender>>(MockBehavior.Strict);

            mockBookingInfoRepository.Setup(repository => repository.Add(It.IsAny<BookingInfo>()))
                .Callback((BookingInfo bookingInfo) =>
                {
                    maxBookingInfoId++;
                    bookingInfo.Id = maxBookingInfoId;
                    actualBookingInfoRepositoryData.Add(bookingInfo);
                });
            mockBookingInfoRepository.Setup(repository => repository.Delete(It.IsAny<int>()))
                .Callback((int id) =>
                {
                    actualBookingInfoRepositoryData.RemoveAll(elem => elem.Id == id);
                });
            
            // Act
            AvailabilityInfoSender infoSender = new AvailabilityInfoSender(
                mockHotelRepository.Object,
                mockRoomRepository.Object,
                mockBookingInfoRepository.Object, 
                mockCalendarParser.Object, 
                mockHttpClientFactory.Object, 
                mockLogger.Object);
            infoSender.SaveChanges(changes);
            
            // Assert
            Assert.Equal(expectedBookingInfoRepositoryData.Count, actualBookingInfoRepositoryData.Count);
            for (int i = 0; i < expectedBookingInfoRepositoryData.Count; i++)
            {
                BookingInfo currExpectedBookingInfo = expectedBookingInfoRepositoryData[i];
                BookingInfo currActualBookingInfo = actualBookingInfoRepositoryData[i];
                
                Assert.Equal(currExpectedBookingInfo.Id, currActualBookingInfo.Id);
                Assert.Equal(currExpectedBookingInfo.StartBooking, currActualBookingInfo.StartBooking);
                Assert.Equal(currExpectedBookingInfo.EndBooking, currActualBookingInfo.EndBooking);
                Assert.Equal(currExpectedBookingInfo.RoomId, currActualBookingInfo.RoomId);
            }
        }
    }
}