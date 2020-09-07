using CalendarIntegrationCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CalendarIntegrationCore.Services
{
    public class AvailabilityInfoDataProcessor: IAvailabilityInfoDataProcessor
    {
        
        /// <summary>
        /// Метод ищет отличия между двумя списками объектов BookingInfo и возвращает их в виде объекта BookingInfoChanges. 
        /// Если какой-либо из объектов содержится в newAvailabilityInfo, но не содержится в initialAvailabilityInfo, он 
        /// добавляется в BookingInfoChanges.AddedBookingInfo. В противоположном случае, он добавляется в BookingInfoChanges.RemovedBookingInfo
        /// </summary>
        /// <param name="newAvailabilityInfo">Новые данные о занятости комнаты</param>
        /// <param name="initialAvailabilityInfo">Изначальные данные о занятости комнаты</param>
        /// <returns>Объект BookingInfoChanges, содержащий информацию о различии изначальных и новых данных</returns>
        public BookingInfoChanges GetChanges(List<BookingInfo> newAvailabilityInfo, List<BookingInfo> initialAvailabilityInfo)
        {
            BookingInfoChanges changes = new BookingInfoChanges();
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
        /// <param name="tlApiCode">TLApiCode комнаты, для которой вычисляется информация
        /// о доступности комнаты в неуказанные дни</param>
        /// <returns>
        /// Список, недостающие даты в котором заполнены 
        /// </returns>
        public List<AvailabilityStatusMessage> FillGapsInDates(
            List<AvailabilityStatusMessage> bookingInfoForOccupiedRooms,
            string tlApiCode)
        {
            List<AvailabilityStatusMessage> result = new List<AvailabilityStatusMessage>();

            if (bookingInfoForOccupiedRooms.Count <= 0)
            {
                return result;
            }
            if (bookingInfoForOccupiedRooms[0].StartDate > DateTime.Now)
            {
                result.Add(new AvailabilityStatusMessage
                {
                    StartDate = DateTime.Today,
                    EndDate = bookingInfoForOccupiedRooms[0].StartDate,
                    RoomId = bookingInfoForOccupiedRooms[0].RoomId,
                    State = BookingLimitType.Available,
                    TLApiCode = tlApiCode
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
                else if (currAvailabilityStatusMessage.RoomId != nextAvailabilityStatusMessage.RoomId)
                {
                    throw new AvailabilityInfoDataProcessorException("RoomId values must be the same for list items");
                }
                else
                {
                    result.Add(currAvailabilityStatusMessage);
                    result.Add(new AvailabilityStatusMessage
                    {
                        StartDate = currAvailabilityStatusMessage.EndDate,
                        EndDate = nextAvailabilityStatusMessage.StartDate,
                        RoomId = currAvailabilityStatusMessage.RoomId,
                        State = BookingLimitType.Available,
                        TLApiCode = tlApiCode
                    });
                }
            }
            result.Add(bookingInfoForOccupiedRooms.Last());
            return result;
        }

        /// <summary>
        /// Данный метод создает список объектов AvailabilityStatusMessage, используя объект BookingInfoChanges.
        /// Для элементов списка BookingInfoChanges.RemovedBookingInfo значение AvailabilityStatusMessage.State
        /// устанавливается в Available, в обратном случае - AvailabilityStatusMessage.State устанавливается в
        /// Occupied
        /// </summary>
        /// <param name="infoChanges">Объект типа BookinfInfoChanges, на основе которого создается список</param>
        /// <param name="tlApiCode">TLApiCode комнаты, для которой передется информация о занятости</param>
        /// <returns>Список объектов типа AvailabilityStatusMessage</returns>
        public List<AvailabilityStatusMessage> CreateAvailabilityStatusMessages(
            BookingInfoChanges infoChanges,
            string tlApiCode)
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
                    TLApiCode = tlApiCode
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
                    TLApiCode = tlApiCode
                });
            }
            return result;
        }
    }
}
