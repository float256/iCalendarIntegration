using CalendarIntegrationCore.Models;
using CalendarIntegrationCore.Services.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace CalendarIntegrationCore.Services
{
    public class AvailabilityInfoSaver: IAvailabilityInfoSaver
    {
        private readonly IBookingInfoRepository _bookingInfoRepository;
        private readonly IAvailabilityStatusMessageQueue _availabilityStatusMessageQueue;
        private readonly IAvailabilityInfoDataProcessor _dataProcessor;
        
        public AvailabilityInfoSaver(
            IBookingInfoRepository bookingInfoRepository, 
            IAvailabilityStatusMessageQueue availabilityStatusMessageQueue,
            IAvailabilityInfoDataProcessor dataProcessor)
        {
            _bookingInfoRepository = bookingInfoRepository;
            _availabilityStatusMessageQueue = availabilityStatusMessageQueue;
            _dataProcessor = dataProcessor;
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
        /// <param name="isFillGaps">Если значение равно true, то промежутки между датами заполняются информацией
        /// о доступности комнаты</param>
        /// <param name="upperBoundForLoadedDatesInDays">Верхняя граница для присваеваемых значений. Если запись о
        /// бронировании находится после даты DateTime.Now.Add(upperBoundForLoadedDatesInDays), то эта запись удаляется
        /// из таблицы booking_info. Также она не сохраняется в availability_status_message. Если значение меньше нуля,
        /// то верхняя граница не устанавливается</param>
        public void AddAllBookingInfoForRoomInQueue(Room room, bool isFillGaps = false, 
            int upperBoundForLoadedDatesInDays = 730)
        {
            List<BookingInfo> allBookingInfoForRoom = _bookingInfoRepository.GetByRoomId(room.Id);
            List<AvailabilityStatusMessage> dateChangeStatusesForRoom = new List<AvailabilityStatusMessage>();
            List<BookingInfo> bookingInfosForDeleting = new List<BookingInfo>();
            foreach (BookingInfo bookingInfo in allBookingInfoForRoom)
            {
                if ((DateTime.Now.Add(TimeSpan.FromDays(upperBoundForLoadedDatesInDays)) > bookingInfo.StartBooking) &&
                    (upperBoundForLoadedDatesInDays > 0))
                {
                    bookingInfosForDeleting.Add(bookingInfo);
                }
                else
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
            foreach (BookingInfo bookingInfo in bookingInfosForDeleting)
            {
                _bookingInfoRepository.Delete(bookingInfo);
            }
            if (isFillGaps)
            {
                dateChangeStatusesForRoom = _dataProcessor.FillGapsInDates(
                    dateChangeStatusesForRoom, 
                    room.TLApiCode);
            }
            _availabilityStatusMessageQueue.EnqueueMultiple(dateChangeStatusesForRoom);
        }
    }
}
