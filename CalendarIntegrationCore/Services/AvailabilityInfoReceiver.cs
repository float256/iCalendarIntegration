using CalendarIntegrationCore.Models;
using CalendarIntegrationCore.Services.Repositories;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;

namespace CalendarIntegrationCore.Services
{
    public class AvailabilityInfoReceiver : IAvailabilityInfoReceiver
    {

        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger _logger;

        public AvailabilityInfoReceiver (
            IHttpClientFactory httpClientFactory,
            ILogger<AvailabilityInfoReceiver> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        /// <summary>
        /// Данный метод получает по URL календарь и возвращает его в виде строки
        /// </summary>
        /// <param name="url">URL адрес календаря</param>
        /// <param name="cancelToken">Токен отмены задачи</param>
        /// <returns>Строка, содержащая календарь</returns>
        public string GetCalendarByUrl(string url, CancellationToken cancelToken)
        {
            if (cancelToken.IsCancellationRequested)
            {
                return string.Empty;
            }

            HttpClient httpClient = _httpClientFactory.CreateClient();
            HttpResponseMessage response;
            try
            {
                response = httpClient.GetAsync(url, cancelToken).Result;
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    return response.Content.ReadAsStringAsync().Result;
                }
                else
                {
                    return string.Empty;
                }
            }
            catch (Exception exception)
            {
                _logger.LogError($"Error occurred while trying to get the calendar from the URL. Exception name: {exception.GetType().Name}; Exception message: {exception.Message}");
                return string.Empty;
            }
        }
    }
}
