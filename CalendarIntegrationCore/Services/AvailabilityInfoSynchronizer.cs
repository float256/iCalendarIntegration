using CalendarIntegrationCore.Models;
using CalendarIntegrationCore.Services.Repositories;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using CalendarIntegrationCore.Services.DataProcessing;
using CalendarIntegrationCore.Services.DataRetrieving;
using CalendarIntegrationCore.Services.DataSaving;
using CalendarIntegrationCore.Services.StatusSaving;

namespace CalendarIntegrationCore.Services
{
    public class AvailabilityInfoSynchronizer: IAvailabilityInfoSynchronizer
    {
        private readonly IHotelRepository _hotelRepository;
        private readonly IRoomRepository _roomRepository;
        private readonly IBookingInfoRepository _bookingInfoRepository;
        
        private readonly IBookingInfoSaver _infoSaver;
        private readonly IAvailabilityInfoReceiver _infoReceiver;
        private readonly IBookingInfoDataProcessor _dataProcessor;
        private readonly IAvailabilityMessageConverter _messageConverter;
        
        private readonly ICalendarParser _calendarParser;
        private readonly IAvailabilityStatusMessageQueue _queue;
        private readonly ILogger _logger;

        private readonly IRoomUploadingStatusSaver _roomUploadingStatusSaver;
        
        public AvailabilityInfoSynchronizer(
            IHotelRepository hotelRepository,
            IRoomRepository roomRepository,
            IBookingInfoSaver infoSaver,
            IAvailabilityInfoReceiver infoReceiver,
            IBookingInfoDataProcessor dataProcessor,
            IBookingInfoRepository bookingInfoRepository,
            IAvailabilityStatusMessageQueue queue,
            ICalendarParser calendarParser,
            ILogger<AvailabilityInfoSynchronizer> logger,
            IAvailabilityMessageConverter messageConverter,
            IRoomUploadingStatusSaver roomUploadStatusSaver)
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
            _messageConverter = messageConverter;
            _roomUploadingStatusSaver = roomUploadStatusSaver;
        }

        /// <summary>
        /// Данный метод получает информацию о занятости для каждой комнаты по URL, указанном в Room.Url,
        /// в виде строки с календарем, парсит календарь, после чего сохраняет изменения в БД.
        /// </summary>
        /// <param name="cancelToken">Токен отмены задачи</param>
        public async Task ProcessAllInfo(CancellationToken cancelToken)
        {
            foreach (Hotel currHotel in _hotelRepository.GetAll())
            {
                foreach (Room currRoom in _roomRepository.GetByHotelId(currHotel.Id))
                {
                    string calendar;
                    RoomUploadStatus newRoomUploadStatus;

                    if (cancelToken.IsCancellationRequested)
                    {
                        return;
                    }

                    try
                    {
                        calendar = await _infoReceiver.GetCalendarByUrl(currRoom.Url, cancelToken);
                    }
                    catch (HttpRequestException exception)
                    {
                        _roomUploadingStatusSaver.SetRoomStatus(currRoom.Id, "Calendar Downloading Error", exception.Message);
                        _logger.LogError(exception, "Error occurred while trying to get the calendar from the URL");
                        continue;
                    }
                    
                    List<BookingInfo> newAvailabilityInfo;
                    List<BookingInfo> initialAvailabilityInfo = _bookingInfoRepository.GetByRoomId(currRoom.Id);
                    try
                    {
                        newAvailabilityInfo = _calendarParser.ParseCalendar(calendar, currRoom.Id);
                    }
                    catch (CalendarParserException exception)
                    {
                        _roomUploadingStatusSaver.SetRoomStatus(currRoom.Id, "Calendar Parsing Error", exception.Message);
                        _logger.LogError(exception, "Error occurred while trying to parse the calendar");
                        continue;
                    }
                    
                    BookingInfoChanges changes = _dataProcessor.GetChanges(newAvailabilityInfo, initialAvailabilityInfo);
                    List<AvailabilityStatusMessage> availabilityStatusMessages =
                        _messageConverter.CreateAvailabilityStatusMessages(changes)
                            .OrderBy(elem => elem.StartDate).ToList();
                    _infoSaver.SaveChanges(changes);
                    _queue.EnqueueMultiple(availabilityStatusMessages);

                    _roomUploadingStatusSaver.SetRoomStatus(currRoom.Id, "OK",  "Successful uploading");
                }
            }
        }
    }
}
