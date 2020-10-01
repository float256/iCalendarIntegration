using System;
using System.Collections.Generic;
using System.Linq;
using CalendarIntegrationCore.Models;
using Microsoft.Extensions.Options;

namespace CalendarIntegrationCore.Services.DataProcessing
{
    public class AvailabilityMessageDataProcessor: IAvailabilityMessageDataProcessor
    {
        private readonly ITodayBoundary _todayBoundary;
        private readonly int _upperBoundForLoadedDates;
        
        public AvailabilityMessageDataProcessor(ITodayBoundary todayBoundary, IOptions<CommonOptions> options)
        {
            _todayBoundary = todayBoundary;
            _upperBoundForLoadedDates = options.Value.UpperBoundForLoadedDatesInDays;
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
        public List<AvailabilityStatusMessage> FillGapsInDates(
            List<AvailabilityStatusMessage> availabilityMessagesForOccupiedRooms,
            int roomId)
        {
            List<AvailabilityStatusMessage> result = new List<AvailabilityStatusMessage>();
            DateTime autofillBoundaryDate = _todayBoundary.GetBoundaryDate(_upperBoundForLoadedDates); 
            
            if (availabilityMessagesForOccupiedRooms.Count == 0)
            {
                if (_upperBoundForLoadedDates != 0)
                {
                    result.Add(new AvailabilityStatusMessage
                    {
                        StartDate = DateTime.Today.AddDays(-1),
                        EndDate = autofillBoundaryDate.AddDays(+1),
                        RoomId = roomId,
                        State = BookingLimitType.Available,
                    });                    
                }
                return result;
            }
            if (_todayBoundary.IsFutureDate(availabilityMessagesForOccupiedRooms[0].StartDate))
            {
                result.Add(new AvailabilityStatusMessage
                {
                    StartDate = DateTime.Today,
                    EndDate = availabilityMessagesForOccupiedRooms[0].StartDate,
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
                    throw new AvailabilityInfoDataProcessorException("Intersection of dates is not allowed");
                }
                else
                {
                    result.Add(currAvailabilityStatusMessage);
                    result.Add(new AvailabilityStatusMessage
                    {
                        StartDate = currAvailabilityStatusMessage.EndDate,
                        EndDate = nextAvailabilityStatusMessage.StartDate,
                        RoomId = roomId,
                        State = BookingLimitType.Available,
                    });
                }
            }
            result.Add(availabilityMessagesForOccupiedRooms.Last());
            if (_todayBoundary.IsWithinSpecifiedLimits(
                availabilityMessagesForOccupiedRooms.Last().EndDate,
                autofillBoundaryDate))
            {
                result.Add(new AvailabilityStatusMessage
                {
                    StartDate = availabilityMessagesForOccupiedRooms.Last().EndDate,
                    EndDate = autofillBoundaryDate,
                    RoomId = roomId,
                    State = BookingLimitType.Available,
                });
            }
            return result;
        }

        /// <summary>
        /// Данный метод создает список объектов AvailabilityStatusMessage, используя объект BookingInfoChanges.
        /// Для элементов списка BookingInfoChanges.RemovedBookingInfo значение AvailabilityStatusMessage.State
        /// устанавливается в Available, в обратном случае - AvailabilityStatusMessage.State устанавливается в
        /// Occupied
        /// </summary>
        /// <param name="infoChanges">Объект типа BookinfInfoChanges, на основе которого создается список</param>
        public List<AvailabilityStatusMessage> CreateAvailabilityStatusMessages(BookingInfoChanges infoChanges)
        {
            List<AvailabilityStatusMessage> result = new List<AvailabilityStatusMessage>();
            foreach (BookingInfo bookingInfo in infoChanges.RemovedBookingInfo)
            {
                result.Add(new AvailabilityStatusMessage
                {
                    RoomId = bookingInfo.RoomId,
                    StartDate = bookingInfo.StartBooking,
                    EndDate = bookingInfo.EndBooking,
                    State = BookingLimitType.Available,
                });
            }
            foreach (BookingInfo bookingInfo in infoChanges.AddedBookingInfo)
            {
                result.Add(new AvailabilityStatusMessage
                {
                    RoomId = bookingInfo.RoomId,
                    StartDate = bookingInfo.StartBooking,
                    EndDate = bookingInfo.EndBooking,
                    State = BookingLimitType.Occupied,
                });
            }
            return result;
        }
    }
}
