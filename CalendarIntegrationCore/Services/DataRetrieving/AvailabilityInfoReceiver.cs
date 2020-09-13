using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using Microsoft.Extensions.Logging;

namespace CalendarIntegrationCore.Services.DataRetrieving
{
    public class AvailabilityInfoReceiver : IAvailabilityInfoReceiver
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public AvailabilityInfoReceiver (IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
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
    }
}
