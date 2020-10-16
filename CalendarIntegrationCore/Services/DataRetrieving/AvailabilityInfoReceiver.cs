using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace CalendarIntegrationCore.Services.DataRetrieving
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
        public async Task<string> GetCalendarByUrl(string url, CancellationToken cancelToken)
        {
            HttpClient httpClient = _httpClientFactory.CreateClient();
            
            var response = await httpClient.GetAsync(url, cancelToken);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }
    }
}
