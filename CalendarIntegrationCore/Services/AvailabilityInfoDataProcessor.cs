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
    }
}
