﻿using System;
using System.Collections.Generic;
using System.Linq;
using CalendarIntegrationCore.Models;
using Microsoft.Extensions.Options;

namespace CalendarIntegrationCore.Services.DataProcessing
{
    public class AvailabilityMessageConverter: IAvailabilityMessageConverter
    {
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
                    EndDate = bookingInfo.EndBooking.AddDays(-1),
                    State = BookingLimitType.Available,
                });
            }
            foreach (BookingInfo bookingInfo in infoChanges.AddedBookingInfo)
            {
                result.Add(new AvailabilityStatusMessage
                {
                    RoomId = bookingInfo.RoomId,
                    StartDate = bookingInfo.StartBooking,
                    EndDate = bookingInfo.EndBooking.AddDays(-1),
                    State = BookingLimitType.Occupied,
                });
            }
            return result;
        }

        /// <summary>
        /// Данный метод создает объект AvailabilityStatusMessage на основе переданных данных. Поле State полученного
        /// объекта равен BookingLimitType.Occupied
        /// </summary>
        /// <param name="bookingInfo">Объект типа BookingInfo, из которого берутся поля StartDate, EndDate, RoomId</param>
        /// <returns>Объект типа AvailabilityStatusMessage, созданный на основе переданных аргументов</returns>
        public AvailabilityStatusMessage CreateAvailabilityStatusMessageForBookingInfo(BookingInfo bookingInfo)
        {
            return new AvailabilityStatusMessage
            {
                StartDate = bookingInfo.StartBooking,
                EndDate = bookingInfo.EndBooking.AddDays(-1),
                RoomId = bookingInfo.RoomId,
                State = BookingLimitType.Occupied,
            };
        }
    }
}
