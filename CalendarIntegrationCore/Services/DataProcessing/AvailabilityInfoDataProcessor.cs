using System;
using System.Collections.Generic;
using System.Linq;
using CalendarIntegrationCore.Models;
using Microsoft.Extensions.Options;

namespace CalendarIntegrationCore.Services.DataProcessing
{
    public class AvailabilityInfoDataProcessor: IAvailabilityInfoDataProcessor
    {
        private readonly int _autofillRangeInDays;
        
        public AvailabilityInfoDataProcessor(IOptions<AvailabilityInfoDataProcessorOptions> options)
        {
            _autofillRangeInDays = options.Value.AutofillRangeInDays;
        }
        
        /// <summary>
        /// Метод ищет отличия между двумя списками объектов BookingInfo и возвращает их в виде объекта BookingInfoChanges. 
        /// Если какой-либо из объектов содержится в newBookingInfos, но не содержится в initialBookingInfos, он 
        /// добавляется в BookingInfoChanges.AddedBookingInfo. В противоположном случае, он добавляется в BookingInfoChanges.RemovedBookingInfo
        /// </summary>
        /// <param name="newBookingInfos">Новые данные о занятости комнаты</param>
        /// <param name="initialBookingInfos">Изначальные данные о занятости комнаты</param>
        /// <returns>Объект BookingInfoChanges, содержащий информацию о различии изначальных и новых данных</returns>
        public BookingInfoChanges GetChanges(List<BookingInfo> newBookingInfos, List<BookingInfo> initialBookingInfos)
        {
            BookingInfoChanges changes = new BookingInfoChanges();
            List<BookingInfo> initialBookingInfoCopy = initialBookingInfos.GetRange(0, initialBookingInfos.Count);

            foreach (BookingInfo newBookingInfo in newBookingInfos)
            {
                bool isFoundMatchingInitialBookingInfo = false;
                for (int i = 0; i < initialBookingInfoCopy.Count; i++)
                {
                    BookingInfo initialBookingInfo = initialBookingInfoCopy[i];
                    isFoundMatchingInitialBookingInfo = (initialBookingInfo.RoomId == newBookingInfo.RoomId) &&
                                                        (initialBookingInfo.StartBooking == newBookingInfo.StartBooking) &&
                                                        (initialBookingInfo.EndBooking == newBookingInfo.EndBooking);
                    if (isFoundMatchingInitialBookingInfo)
                    {
                        initialBookingInfoCopy.Remove(initialBookingInfo);
                        break;
                    }
                }
                if (!isFoundMatchingInitialBookingInfo)
                {
                    changes.AddedBookingInfo.Add(newBookingInfo);
                }
            }
            foreach (BookingInfo remainingInitialBookingInfo in initialBookingInfoCopy)
            {
                changes.RemovedBookingInfo.Add(remainingInitialBookingInfo);
            }
            changes.AddedBookingInfo = changes.AddedBookingInfo.OrderBy(bookingInfo => bookingInfo.StartBooking).ToList();
            changes.RemovedBookingInfo = changes.RemovedBookingInfo.OrderBy(bookingInfo => bookingInfo.StartBooking).ToList();
            return changes;
        }

        /// <summary>
        /// Метод добавляет в список информацию о доступности комнаты в неуказанные дни
        /// </summary>
        /// <param name="bookingInfoForOccupiedRooms">
        /// Список, содержащий информацию о доступности комнаты в определенные промежутки
        /// </param>
        /// <param name="roomId">Id комнаты, для которой заполняются пропуски в датах</param>
        /// <returns>
        /// Список, недостающие даты в котором заполнены 
        /// </returns>
        public List<AvailabilityStatusMessage> FillGapsInDates(
            List<AvailabilityStatusMessage> bookingInfoForOccupiedRooms,
            int roomId)
        {
            List<AvailabilityStatusMessage> result = new List<AvailabilityStatusMessage>();
            DateTime autofillRangeDate = DateTime.Now.Add(TimeSpan.FromDays(_autofillRangeInDays)); 
            
            if (bookingInfoForOccupiedRooms.Count == 0)
            {
                if (_autofillRangeInDays != 0)
                {
                    result.Add(new AvailabilityStatusMessage
                    {
                        StartDate = DateTime.Today.AddDays(+1),
                        EndDate = autofillRangeDate.AddDays(-1),
                        RoomId = roomId,
                        State = BookingLimitType.Available,
                    });                    
                }
                return result;
            }
            if (bookingInfoForOccupiedRooms[0].StartDate > DateTime.Now)
            {
                result.Add(new AvailabilityStatusMessage
                {
                    StartDate = DateTime.Today,
                    EndDate = bookingInfoForOccupiedRooms[0].StartDate,
                    RoomId = roomId,
                    State = BookingLimitType.Available,
                });
            }
            for (int i = 0; i < bookingInfoForOccupiedRooms.Count - 1; i++)
            {
                AvailabilityStatusMessage currAvailabilityStatusMessage = bookingInfoForOccupiedRooms[i];
                AvailabilityStatusMessage nextAvailabilityStatusMessage = bookingInfoForOccupiedRooms[i + 1];

                if (currAvailabilityStatusMessage.EndDate > 
                    nextAvailabilityStatusMessage.StartDate)
                {
                    throw new AvailabilityInfoDataProcessorException("Intersection of dates is not allowed");
                }
                else
                {
                    result.Add(currAvailabilityStatusMessage);
                    result.Add(new AvailabilityStatusMessage
                    {
                        StartDate = currAvailabilityStatusMessage.EndDate.AddDays(+1),
                        EndDate = nextAvailabilityStatusMessage.StartDate.AddDays(-1),
                        RoomId = roomId,
                        State = BookingLimitType.Available,
                    });
                }
            }
            result.Add(bookingInfoForOccupiedRooms.Last());
            if (bookingInfoForOccupiedRooms.Last().EndDate < autofillRangeDate)
            {
                result.Add(new AvailabilityStatusMessage
                {
                    StartDate = bookingInfoForOccupiedRooms.Last().EndDate.AddDays(+1),
                    EndDate = autofillRangeDate,
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
        /// <returns>Список объектов типа AvailabilityStatusMessage</returns>
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
