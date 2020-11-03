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

namespace CalendarIntegrationCore.Services
{
    public class AvailabilityInfoSynchronizer: IAvailabilityInfoSynchronizer, IObservable<RoomUploadStatus>
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
        private readonly IRoomUploadStatusRepository _roomUploadStatusRepository;
        
        private readonly List<IObserver<RoomUploadStatus>> _observers;
        
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
            IRoomUploadStatusRepository roomUploadStatusRepository,
            List<IObserver<RoomUploadStatus>> observers)
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
            _roomUploadStatusRepository = roomUploadStatusRepository;
            _observers = observers;
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
                        newRoomUploadStatus= new RoomUploadStatus
                        {
                            Message = exception.Message,
                            Status = "Calendar Downloading Error",
                            RoomId = currRoom.Id
                        };
                        
                        SendAllObservers(newRoomUploadStatus);
                        _roomUploadStatusRepository.SetStatus(newRoomUploadStatus);
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
                        newRoomUploadStatus = new RoomUploadStatus
                        {
                            Message = exception.Message,
                            Status = "Calendar Parsing Error",
                            RoomId = currRoom.Id
                        };
                        
                        _roomUploadStatusRepository.SetStatus(newRoomUploadStatus);
                        SendAllObservers(newRoomUploadStatus);
                        _logger.LogError(exception, "Error occurred while trying to parse the calendar");
                        continue;
                    }
                    
                    BookingInfoChanges changes = _dataProcessor.GetChanges(newAvailabilityInfo, initialAvailabilityInfo);
                    List<AvailabilityStatusMessage> availabilityStatusMessages =
                        _messageConverter.CreateAvailabilityStatusMessages(changes)
                            .OrderBy(elem => elem.StartDate).ToList();
                    _infoSaver.SaveChanges(changes);
                    _queue.EnqueueMultiple(availabilityStatusMessages);

                    newRoomUploadStatus = new RoomUploadStatus
                    {
                        Message = "Successful uploading",
                        Status = "OK",
                        RoomId = currRoom.Id
                    };
                    _roomUploadStatusRepository.SetStatus(newRoomUploadStatus);
                    SendAllObservers(newRoomUploadStatus);
                }
            }
        }

        public IDisposable Subscribe(IObserver<RoomUploadStatus> observer)
        {
            _observers.Add(observer);
            return new Unsubscriber(_observers, observer);    
        }

        private void SendAllObservers(RoomUploadStatus newRoomUploadStatus)
        {
            foreach (var observer in _observers)
            {
                observer.OnNext(newRoomUploadStatus);
            }
        }
        
        private class Unsubscriber : IDisposable
        {
            private readonly List<IObserver<RoomUploadStatus>> _observers;
            private readonly IObserver<RoomUploadStatus> _observer;

            public Unsubscriber(List<IObserver<RoomUploadStatus>> observers, IObserver<RoomUploadStatus> observer)
            {
                this._observers = observers;
                this._observer = observer;
            }

            public void Dispose()
            {
                if (_observer != null)
                {
                    _observers.Remove(_observer);
                }
            }
        }
    }
}
