using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using CalendarIntegrationCore.Models;
using CalendarIntegrationCore.Services.Repositories;
using Microsoft.Extensions.Logging;

namespace CalendarIntegrationCore.Services
{
    public class AvailabilityInfoSender: IAvailabilityInfoSender
    {
        private readonly IHotelRepository _hotelRepository;
        private readonly IRoomRepository _roomRepository;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IBookingInfoRepository _bookingInfoRepository;
        private readonly ICalendarParser _calendarParser;
        private readonly ILogger _logger;
        
        public AvailabilityInfoSender(
            IHotelRepository hotelRepository, 
            IRoomRepository roomRepository, 
            IBookingInfoRepository bookingInfoRepository, 
            ICalendarParser calendarParser, 
            IHttpClientFactory httpClientFactory,
            ILogger<AvailabilityInfoSender> logger)
        {
            _hotelRepository = hotelRepository;
            _roomRepository = roomRepository;
            _bookingInfoRepository = bookingInfoRepository;
            _calendarParser = calendarParser;
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }
        
        public void SaveAndSendAllInfo()
        {
            foreach (Hotel currHotel in _hotelRepository.GetAll())
            {
                foreach (Room currRoom in _roomRepository.GetByHotelId(currHotel.Id))
                {
                    string calendar = GetCalendarByUrl(currRoom.Url);
                    BookingInfoChanges changes = GetChanges(calendar, currRoom.Id);
                    SaveChanges(changes);
                }
            }
        }

        public string GetCalendarByUrl(string url)
        {
            HttpClient httpClient = _httpClientFactory.CreateClient();
            HttpResponseMessage response;
            try
            {
                response = httpClient.GetAsync(url).Result;
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

        public void SaveChanges(BookingInfoChanges changes)
        {
            foreach (BookingInfo bookingInfo in changes.AddedBookingInfo)
            {
                _bookingInfoRepository.Add(bookingInfo);
            }

            foreach (BookingInfo bookingInfo in changes.RemovedBookingInfo)
            {
                _bookingInfoRepository.Delete(bookingInfo.Id);
            }
        }
        
        public BookingInfoChanges GetChanges(string calendar, int roomId)
        {
            List<BookingInfo> newAvailabilityInfo;
            List<BookingInfo> initialAvailabilityInfo = _bookingInfoRepository.GetByRoomId(roomId);
            BookingInfoChanges changes = new BookingInfoChanges();
            try
            {
                newAvailabilityInfo = _calendarParser.ParseCalendar(calendar, roomId);
            }
            catch (Exception exception)
            {
                _logger.LogError($"{exception.GetType().Name}: {exception.Message}");
                return new BookingInfoChanges();
            }

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
            return changes;
        }
    }
}