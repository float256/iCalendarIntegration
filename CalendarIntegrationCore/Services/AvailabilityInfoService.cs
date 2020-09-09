using CalendarIntegrationCore.Models;
using CalendarIntegrationCore.Services.Repositories;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CalendarIntegrationCore.Services.DataDownloading;
using CalendarIntegrationCore.Services.DataProcessing;
using CalendarIntegrationCore.Services.DataRetrieving;

namespace CalendarIntegrationCore.Services
{
    public class AvailabilityInfoService: IAvailabilityInfoService
    {
        private readonly IHotelRepository _hotelRepository;
        private readonly IRoomRepository _roomRepository;
        private readonly IBookingInfoRepository _bookingInfoRepository;
        
        private readonly IAvailabilityInfoSaver _infoSaver;
        private readonly IAvailabilityInfoReceiver _infoReceiver;
        private readonly IAvailabilityInfoDataProcessor _dataProcessor;
        
        private readonly ICalendarParser _calendarParser;
        private readonly IAvailabilityStatusMessageQueue _queue;
        private readonly ILogger _logger;

        public AvailabilityInfoService(
            IHotelRepository hotelRepository,
            IRoomRepository roomRepository,
            IAvailabilityInfoSaver infoSaver,
            IAvailabilityInfoReceiver infoReceiver,
            IAvailabilityInfoDataProcessor dataProcessor,
            IBookingInfoRepository bookingInfoRepository,
            IAvailabilityStatusMessageQueue queue,
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
            _queue = queue;
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
                            _logger.LogError(exception, "Error occurred while trying to parse the calendar");
                            continue;
                        }
                        BookingInfoChanges changes = _dataProcessor.GetChanges(newAvailabilityInfo, initialAvailabilityInfo);
                        List<AvailabilityStatusMessage> availabilityStatusMessages =
                            _dataProcessor.CreateAvailabilityStatusMessages(changes, currRoom.TLApiCode)
                                .OrderBy(elem => elem.StartDate).ToList();
                        _infoSaver.SaveChanges(changes);

                        if (initialAvailabilityInfo.Count == 0)
                        {
                            try
                            {
                                availabilityStatusMessages = _dataProcessor.FillGapsInDates(
                                    availabilityStatusMessages, currRoom.Id);
                            }
                            catch (Exception e)
                            {
                                _logger.LogError(e, "Error occurred while trying to fill gaps in dates");
                            }
                        }
                        _queue.EnqueueMultiple(availabilityStatusMessages);
                    }
                }
            }
        }
    }
}
