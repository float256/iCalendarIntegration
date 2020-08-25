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
        private readonly IBookingInfoRepository _bookingInfoRepository;
        private readonly ILogger _logger;

        public AvailabilityInfoReceiver (
            IBookingInfoRepository bookingInfoRepository,
            IHttpClientFactory httpClientFactory,
            ILogger<AvailabilityInfoReceiver> logger)
        {
            _bookingInfoRepository = bookingInfoRepository;
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        public string GetCalendarByUrl(string url, CancellationToken cancelToken)
        {
            if (cancelToken.IsCancellationRequested)
            {
                return String.Empty;
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
                    return String.Empty;
                }
            }
            catch (Exception exception)
            {
                _logger.LogError($"{exception.GetType().Name}: {exception.Message}");
                return String.Empty;
            }
        }

        public BookingInfoChanges GetChanges(List<BookingInfo> newAvailabilityInfo, List<BookingInfo> initialAvailabilityInfo)
        {
            BookingInfoChanges changes = new BookingInfoChanges();
            foreach (BookingInfo newBookingInfo in newAvailabilityInfo)
            {
                bool isFoundMatchingInitialBookingInfo = false;
                for (int i = 0; i < initialAvailabilityInfo.Count; i++)
                {
                    BookingInfo initialBookingInfo = initialAvailabilityInfo[i];
                    isFoundMatchingInitialBookingInfo = (initialBookingInfo.RoomId == newBookingInfo.RoomId) &&
                                                        (initialBookingInfo.StartBooking == newBookingInfo.StartBooking) &&
                                                        (initialBookingInfo.EndBooking == newBookingInfo.EndBooking);
                    if (isFoundMatchingInitialBookingInfo)
                    {
                        initialAvailabilityInfo.Remove(initialBookingInfo);
                        break;
                    }
                }
                if (!isFoundMatchingInitialBookingInfo)
                {
                    changes.AddedBookingInfo.Add(newBookingInfo);
                }
            }
            foreach (BookingInfo remainingInitialBookingInfo in initialAvailabilityInfo)
            {
                changes.RemovedBookingInfo.Add(remainingInitialBookingInfo);
            }
            changes.AddedBookingInfo = changes.AddedBookingInfo.OrderBy(bookingInfo => bookingInfo.StartBooking).ToList();
            changes.RemovedBookingInfo = changes.RemovedBookingInfo.OrderBy(bookingInfo => bookingInfo.StartBooking).ToList();
            return changes;
        }
    }
}
