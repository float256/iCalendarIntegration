using System;
using System.Collections.Generic;
using CalendarIntegrationCore.Models;
using CalendarIntegrationCore.Services.DataProcessing;
using CalendarIntegrationCore.Services.DataRetrieving;
using CalendarIntegrationCore.Services.Repositories;
using Microsoft.Extensions.Options;

namespace CalendarIntegrationCore.Services.DataDownloading
{
    public class AvailabilityInfoSaver: IAvailabilityInfoSaver
    {
        private readonly IBookingInfoRepository _bookingInfoRepository;
        private readonly IAvailabilityStatusMessageQueue _availabilityStatusMessageQueue;
        private readonly IAvailabilityMessageDataProcessor _messageDataProcessor;
        private readonly int _upperBoundForLoadedDatesInDays;
        private readonly ITodayBoundary _todayBoundary;
        
        public AvailabilityInfoSaver(
            IBookingInfoRepository bookingInfoRepository, 
            IAvailabilityStatusMessageQueue availabilityStatusMessageQueue,
            IAvailabilityMessageDataProcessor messageDataProcessor,
            IOptions<CommonOptions> options,
            ITodayBoundary todayBoundary)
        {
            _bookingInfoRepository = bookingInfoRepository;
            _availabilityStatusMessageQueue = availabilityStatusMessageQueue;
            _messageDataProcessor = messageDataProcessor;
            _upperBoundForLoadedDatesInDays = options.Value.UpperBoundForLoadedDatesInDays;
            _todayBoundary = todayBoundary;
        }

        /// <summary>
        /// Метод делает изменения в базе данных согласно переданному объекту BookingInfoChanges, а именно, 
        /// сохраняет данные из списка AddedBookingInfo и удаляет данные, связанные со списком RemovedBookingInfo
        /// </summary>
        /// <param name="changes">Объект BookingInfoChanges, содержащий информацию об изменениях, которые нужно сделать в БД</param>
        public void SaveChanges(BookingInfoChanges changes)
        {
            foreach (BookingInfo bookingInfo in changes.AddedBookingInfo)
            {
                _bookingInfoRepository.Add(bookingInfo);
            }

            foreach (BookingInfo bookingInfo in changes.RemovedBookingInfo)
            {
                _bookingInfoRepository.Delete(bookingInfo);
            }
        }

        /// <summary>
        /// Метод добавляет в таблицу availability_status_message всю информацию о доступности указанной комнаты. State каждой
        /// добавленной даты равен BookingLimitType.Occupied, т.к. в базе данных лежит информация только о датах, когда
        /// комната занята 
        /// </summary>
        /// <param name="roomId">Объект комнаты, для которой добавляются значения о доступности в БД</param>
        /// <param name="room"></param>
        /// <param name="isFillGaps">Если значение равно true, то промежутки между датами заполняются информацией
        /// о доступности комнаты</param>
        public void AddAvailabilityMessagesForRoomInQueue(Room room, bool isFillGaps = false)
        {
            List<BookingInfo> allBookingInfoForRoom = _bookingInfoRepository.GetByRoomId(room.Id);
            List<AvailabilityStatusMessage> dateChangeStatusesForRoom = new List<AvailabilityStatusMessage>();
            List<BookingInfo> bookingInfosForDeleting = new List<BookingInfo>();
            DateTime boundaryDate = _todayBoundary.GetBoundaryDate(_upperBoundForLoadedDatesInDays);
            
            foreach (BookingInfo bookingInfo in allBookingInfoForRoom)
            {
                if (_todayBoundary.IsWithinSpecifiedLimits(bookingInfo.StartBooking, boundaryDate))
                {
                    dateChangeStatusesForRoom.Add(new AvailabilityStatusMessage
                    {
                        RoomId = room.Id,
                        StartDate = bookingInfo.StartBooking,
                        EndDate = bookingInfo.EndBooking,
                        State = BookingLimitType.Occupied
                    });                    
                }
            }
            
            if (isFillGaps)
            {
                dateChangeStatusesForRoom = _messageDataProcessor.FillGapsInDates(dateChangeStatusesForRoom, room.Id);
            }
            _availabilityStatusMessageQueue.EnqueueMultiple(dateChangeStatusesForRoom);
        }
    }
}
