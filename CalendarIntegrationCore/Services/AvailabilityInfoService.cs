using CalendarIntegrationCore.Models;
using CalendarIntegrationCore.Services.Repositories;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;

namespace CalendarIntegrationCore.Services
{
    public class AvailabilityInfoService: IAvailabilityInfoService
    {
        private readonly IHotelRepository _hotelRepository;
        private readonly IRoomRepository _roomRepository;
        private readonly IBookingInfoRepository _bookingInfoRepository;
        private readonly ICalendarParser _calendarParser;
        private readonly IAvailabilityInfoSaver _infoSaver;
        private readonly IAvailabilityInfoReceiver _infoReceiver;
        private readonly IAvailabilityInfoDataProcessor _dataProcessor;
        private readonly ILogger _logger;

        public AvailabilityInfoService(
            IHotelRepository hotelRepository,
            IRoomRepository roomRepository,
            IAvailabilityInfoSaver infoSaver,
            IAvailabilityInfoReceiver infoReceiver,
            IAvailabilityInfoDataProcessor dataProcessor,
            IBookingInfoRepository bookingInfoRepository,
            ICalendarParser calendarParser,
            ILogger<AvailabilityInfoService> logger)
        {
            _hotelRepository = hotelRepository;
            _roomRepository = roomRepository;
            _bookingInfoRepository = bookingInfoRepository;
            _infoSaver = infoSaver;
            _infoReceiver = infoReceiver;
            _dataProcessor = dataProcessor;
            _calendarParser = calendarParser;
            _logger = logger;
        }

        /// <summary>
        /// Данный метод получает информацию о занятости для каждой комнаты по URL, указанном в Room.Url,
        /// в виде строки с календарем, парсит календарь, после чего сохраняет изменения в БД.
        /// </summary>
        /// <param name="cancelToken">Токен отмены задачи</param>
        public void ProcessAllInfo(CancellationToken cancelToken)
        {
            if (!cancelToken.IsCancellationRequested)
            {
                foreach (Hotel currHotel in _hotelRepository.GetAll())
                {
                    foreach (Room currRoom in _roomRepository.GetByHotelId(currHotel.Id))
                    {
                        string calendar = _infoReceiver.GetCalendarByUrl(currRoom.Url, cancelToken);
                        List<BookingInfo> newAvailabilityInfo;
                        List<BookingInfo> initialAvailabilityInfo = _bookingInfoRepository.GetByRoomId(currRoom.Id);
                        try
                        {
                            newAvailabilityInfo = _calendarParser.ParseCalendar(calendar, currRoom.Id);
                        }
                        catch (Exception exception)
                        {
                            _logger.LogError($"Error occurred while trying to parse the calendar. Exception name: {exception.GetType().Name}; Exception message: {exception.Message}");
                            continue;
                        }
                        BookingInfoChanges changes = _dataProcessor.GetChanges(newAvailabilityInfo, initialAvailabilityInfo);
                        _infoSaver.SaveChanges(changes);
                    }
                }
            }
        }
    }
}
