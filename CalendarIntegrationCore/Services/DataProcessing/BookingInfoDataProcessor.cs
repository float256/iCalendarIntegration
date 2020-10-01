using System;
using System.Collections.Generic;
using System.Linq;
using CalendarIntegrationCore.Models;
using Microsoft.Extensions.Options;

namespace CalendarIntegrationCore.Services.DataProcessing
{
    public class BookingInfoDataProcessor: IBookingInfoDataProcessor
    {
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

            foreach (BookingInfo currNewBookingInfo in newBookingInfos)
            {
                bool isFoundMatchingInitialBookingInfo = false;
                for (int i = 0; i < initialBookingInfoCopy.Count; i++)
                {
                    BookingInfo currInitialBookingInfo = initialBookingInfoCopy[i];
                    isFoundMatchingInitialBookingInfo = (currInitialBookingInfo.RoomId == currNewBookingInfo.RoomId) &&
                                                        (currInitialBookingInfo.StartBooking == currNewBookingInfo.StartBooking) &&
                                                        (currInitialBookingInfo.EndBooking == currNewBookingInfo.EndBooking);
                    if (isFoundMatchingInitialBookingInfo)
                    {
                        initialBookingInfoCopy.Remove(currInitialBookingInfo);
                        break;
                    }
                }
                if (!isFoundMatchingInitialBookingInfo)
                {
                    changes.AddedBookingInfo.Add(currNewBookingInfo);
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
    }
}
