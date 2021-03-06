﻿using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using CalendarIntegrationCore.Services.DataRetrieving;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using Xunit;

namespace CalendarIntegrationCore.Tests.Services.DataRetrieving
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
                mockHttpClientFactory.Object,
                mockLogger.Object);
            string actualCalendar = availabilityInfoReceiver.GetCalendarByUrl(url, CancellationToken.None).Result;

            // Assert
            Assert.Equal(expectedCalendar, actualCalendar);
        }

        [Fact]
        public void AvailabilityInfoSender_GetCalendarByUrl_ResponseIsNotOK_HttpRequestException()
        {
            // Arrange
            string expectedExceptionMessage = "One or more errors occurred. (Response status code does not indicate success: 404 (Not Found).)";
            string url = "http://example.com";

            Mock<IHttpClientFactory> mockHttpClientFactory = new Mock<IHttpClientFactory>(MockBehavior.Strict);
            Mock<HttpMessageHandler> mockHttpMessageHandler = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            Mock<ILogger<AvailabilityInfoReceiver>> mockLogger = new Mock<ILogger<AvailabilityInfoReceiver>>(MockBehavior.Strict);

            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.NotFound
                });

            HttpClient httpClient = new HttpClient(mockHttpMessageHandler.Object)
            {
                BaseAddress = new Uri(url)
            };
            mockHttpClientFactory.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(httpClient);

            // Act
            AvailabilityInfoReceiver availabilityInfoReceiver = new AvailabilityInfoReceiver(
                mockHttpClientFactory.Object,
                mockLogger.Object);
            AggregateException exception = Assert.Throws<AggregateException>(
                () => availabilityInfoReceiver.GetCalendarByUrl(url, CancellationToken.None).Result);

            // Assert
            Assert.Equal(expectedExceptionMessage, exception.Message);
        }
    }
}
