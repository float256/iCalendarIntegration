using System;
using System.Collections.Generic;
using System.Linq;
using CalendarIntegrationCore.Models;
using CalendarIntegrationCore.Services.DataProcessing;
using CalendarIntegrationCore.Services.Repositories;
using Microsoft.Extensions.Options;

namespace CalendarIntegrationCore.Services.DataSaving
{
    public class RoomAvailabilityInitializationHandler: IRoomAvailabilityInitializationHandler
    {
        private readonly IBookingInfoRepository _bookingInfoRepository;
        private readonly ITodayBoundary _todayBoundary;
        private readonly IAvailabilityStatusMessageQueue _availabilityStatusMessageQueue;
        private readonly int _synchronizationDaysInFuture;

        
        public RoomAvailabilityInitializationHandler(
            IBookingInfoRepository bookingInfoRepository,
            ITodayBoundary todayBoundary,
            IAvailabilityStatusMessageQueue availabilityStatusMessageQueue,
            IOptions<DateSynchronizationCommonOptions> options)
        {
            _bookingInfoRepository = bookingInfoRepository;
            _todayBoundary = todayBoundary;
            _availabilityStatusMessageQueue = availabilityStatusMessageQueue;
            _synchronizationDaysInFuture = options.Value.SynchronizationDaysInFuture;
        }
        
        /// <summary>
        /// Метод добавляет в таблицу availability_status_message всю информацию о доступности указанной комнаты. State каждой
        /// добавленной даты равен BookingLimitType.Occupied, т.к. в базе данных лежит информация только о датах, когда
        /// комната занята 
        /// </summary>
        /// <param name="roomId">Объект комнаты, для которой добавляются значения о доступности в БД</param>
        /// <param name="room"></param>
        public void AddAvailabilityMessagesForRoomInQueue(Room room)
        {
            List<BookingInfo> allBookingInfoForRoom = _bookingInfoRepository.GetByRoomId(room.Id);
            List<AvailabilityStatusMessage> dateChangeStatusesForRoom = new List<AvailabilityStatusMessage>();
            
            foreach (BookingInfo bookingInfo in allBookingInfoForRoom)
            {
                if (bookingInfo.StartBooking <  _todayBoundary.GetMaxDate())
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
            dateChangeStatusesForRoom = FillDateGapsAsAvailable(dateChangeStatusesForRoom, room.Id);
            _availabilityStatusMessageQueue.EnqueueMultiple(dateChangeStatusesForRoom);
        }
        
        /// <summary>
        /// Метод добавляет в список информацию о доступности комнаты в неуказанные дни
        /// </summary>
        /// <param name="availabilityMessagesForOccupiedRooms">
        /// Список, содержащий информацию о доступности комнаты в определенные промежутки
        /// </param>
        /// <param name="roomId">Id комнаты, для которой заполняются пропуски в датах</param>
        /// <returns>
        /// Список, недостающие даты в котором заполнены 
        /// </returns>
        private List<AvailabilityStatusMessage> FillDateGapsAsAvailable(
            List<AvailabilityStatusMessage> availabilityMessagesForOccupiedRooms,
            int roomId)
        {
            List<AvailabilityStatusMessage> result = new List<AvailabilityStatusMessage>();
            
            if (availabilityMessagesForOccupiedRooms.Count == 0)
            {
                result.Add(new AvailabilityStatusMessage
                {
                    StartDate = _todayBoundary.GetMinDate(),
                    EndDate = _todayBoundary.GetMaxDate(),
                    RoomId = roomId,
                    State = BookingLimitType.Available,
                });
                return result;
            }
            if (_todayBoundary.IsFutureDate(availabilityMessagesForOccupiedRooms[0].StartDate))
            {
                result.Add(new AvailabilityStatusMessage
                {
                    StartDate = _todayBoundary.GetMinDate().AddDays(1),
                    EndDate = availabilityMessagesForOccupiedRooms[0].StartDate.AddDays(-1),
                    RoomId = roomId,
                    State = BookingLimitType.Available
                });
            }
            for (int i = 0; i < availabilityMessagesForOccupiedRooms.Count - 1; i++)
            {
                AvailabilityStatusMessage currAvailabilityStatusMessage = availabilityMessagesForOccupiedRooms[i];
                AvailabilityStatusMessage nextAvailabilityStatusMessage = availabilityMessagesForOccupiedRooms[i + 1];

                if (currAvailabilityStatusMessage.EndDate > nextAvailabilityStatusMessage.StartDate)
                {
                    throw new RoomAvailabilityInitializationHandlerException("Intersection of dates is not allowed");
                }
                else
                {
                    result.Add(currAvailabilityStatusMessage);
                    result.Add(new AvailabilityStatusMessage
                    {
                        StartDate = currAvailabilityStatusMessage.EndDate.AddDays(1),
                        EndDate = nextAvailabilityStatusMessage.StartDate.AddDays(-1),
                        RoomId = roomId,
                        State = BookingLimitType.Available,
                    });
                }
            }
            result.Add(availabilityMessagesForOccupiedRooms.Last());
            if (availabilityMessagesForOccupiedRooms.Last().EndDate <  _todayBoundary.GetMaxDate())
            {
                result.Add(new AvailabilityStatusMessage
                {
                    StartDate = availabilityMessagesForOccupiedRooms.Last().EndDate.AddDays(1),
                    EndDate = _todayBoundary.GetMaxDate(),
                    RoomId = roomId,
                    State = BookingLimitType.Available,
                });
            }
            return result;
        }
    }
}