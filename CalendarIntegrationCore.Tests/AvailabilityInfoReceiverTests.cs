using CalendarIntegrationCore.Models;
using CalendarIntegrationCore.Services;
using CalendarIntegrationCore.Services.Repositories;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace CalendarIntegrationCore.Tests
{
    public class AvailabilityInfoReceiverTests
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

            Mock<IBookingInfoRepository> mockBookingInfoRepository = new Mock<IBookingInfoRepository>(MockBehavior.Strict);
            Mock<IHttpClientFactory> mockHttpClientFactory = new Mock<IHttpClientFactory>(MockBehavior.Strict);
            Mock<HttpMessageHandler> mockHttpMessageHandler = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            Mock<ILogger<AvailabilityInfoReceiver>> mockLogger = new Mock<ILogger<AvailabilityInfoReceiver>>(MockBehavior.Strict);

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

            // Act
            AvailabilityInfoReceiver availabilityInfoReceiver = new AvailabilityInfoReceiver(
                mockBookingInfoRepository.Object,
                mockHttpClientFactory.Object,
                mockLogger.Object);
            string actualCalendar = availabilityInfoReceiver.GetCalendarByUrl(url, CancellationToken.None);

            // Assert
            Assert.Equal(expectedCalendar, actualCalendar);
        }

        [Fact]
        public void AvailabilityInfoSender_GetCalendarByUrl_ResponseIsNotOK_StringWithCalendar()
        {
            // Arrange
            string expectedCalendar = String.Empty;
            string url = "http://example.com";

            Mock<IBookingInfoRepository> mockBookingInfoRepository = new Mock<IBookingInfoRepository>(MockBehavior.Strict);
            Mock<IHttpClientFactory> mockHttpClientFactory = new Mock<IHttpClientFactory>(MockBehavior.Strict);
            Mock<HttpMessageHandler> mockHttpMessageHandler = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            Mock<ILogger<AvailabilityInfoReceiver>> mockLogger = new Mock<ILogger<AvailabilityInfoReceiver>>(MockBehavior.Strict);

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

            // Act
            AvailabilityInfoReceiver availabilityInfoReceiver = new AvailabilityInfoReceiver(
                mockBookingInfoRepository.Object,
                mockHttpClientFactory.Object,
                mockLogger.Object);
            string actualCalendar = availabilityInfoReceiver.GetCalendarByUrl(url, CancellationToken.None);

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
            List<BookingInfo> initialAvailabilityInfo = new List<BookingInfo>
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
            List<BookingInfo> newAvailabilityInfo = new List<BookingInfo>
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
            Mock<IHttpClientFactory> mockHttpClientFactory = new Mock<IHttpClientFactory>(MockBehavior.Strict);
            Mock<IBookingInfoRepository> mockBookingInfoRepository = new Mock<IBookingInfoRepository>(MockBehavior.Strict);
            Mock<ILogger<AvailabilityInfoReceiver>> mockLogger = new Mock<ILogger<AvailabilityInfoReceiver>>(MockBehavior.Strict);

            // Act
            AvailabilityInfoReceiver infoSender = new AvailabilityInfoReceiver(
                mockBookingInfoRepository.Object,
                mockHttpClientFactory.Object,
                mockLogger.Object);
            BookingInfoChanges actual = infoSender.GetChanges(newAvailabilityInfo, initialAvailabilityInfo);

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
            List<BookingInfo> newAvailabilityInfo = new List<BookingInfo> { };
            List<BookingInfo> initialAvailabilityInfo = new List<BookingInfo>();
            BookingInfoChanges expected = new BookingInfoChanges();

            Mock<IBookingInfoRepository> mockBookingInfoRepository = new Mock<IBookingInfoRepository>(MockBehavior.Strict);
            Mock<ILogger<AvailabilityInfoReceiver>> mockLogger = new Mock<ILogger<AvailabilityInfoReceiver>>(MockBehavior.Strict);
            Mock<IHttpClientFactory> mockHttpClientFactory = new Mock<IHttpClientFactory>(MockBehavior.Strict);

            // Act
            AvailabilityInfoReceiver infoSender = new AvailabilityInfoReceiver(
                mockBookingInfoRepository.Object,
                mockHttpClientFactory.Object,
                mockLogger.Object);
            BookingInfoChanges actual = infoSender.GetChanges(newAvailabilityInfo, initialAvailabilityInfo);

            // Assert
            Assert.Equal(expected.AddedBookingInfo.Count, actual.AddedBookingInfo.Count);
            Assert.Equal(expected.RemovedBookingInfo.Count, actual.RemovedBookingInfo.Count);
        }
    }
}
