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
using CalendarIntegrationCore.Services.DataRetrieving;
using Xunit;

namespace CalendarIntegrationCore.Tests.Services
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
            AvailabilityInfoReceiver availabilityInfoReceiver = new AvailabilityInfoReceiver(mockHttpClientFactory.Object);
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

            Mock<IHttpClientFactory> mockHttpClientFactory = new Mock<IHttpClientFactory>(MockBehavior.Strict);
            Mock<HttpMessageHandler> mockHttpMessageHandler = new Mock<HttpMessageHandler>(MockBehavior.Strict);

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
            AvailabilityInfoReceiver availabilityInfoReceiver = new AvailabilityInfoReceiver(mockHttpClientFactory.Object);
            string actualCalendar = availabilityInfoReceiver.GetCalendarByUrl(url, CancellationToken.None);

            // Assert
            Assert.Equal(expectedCalendar, actualCalendar);
        }
    }
}
