using System;
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
        /// Данный метод создает объект AvailabilityStatusMessage на основе переданных данных
        /// </summary>
        /// <param name="bookingInfo">Объект типа BookingInfo, из которого берутся поля StartDate, EndDate, RoomId</param>
        /// <param name="state">Информация о занятости комнаты</param>
        /// <param name="addDaysForStartDate">Количество дней, которое добавляется к BookingInfo.StartBooking
        /// при конвертировании поля в StartDate. Также допустимы отрицательные значения</param>
        /// <param name="addDaysForEndDate">Количество дней, которое добавляется к BookingInfo.EndBooking
        /// при конвертировании поля в EndDate. Также допустимы отрицательные значения</param>
        /// <returns>Объект типа AvailabilityStatusMessage, созданный на основе переданных аргументов</returns>
        public AvailabilityStatusMessage CreateAvailabilityStatusMessage(
            BookingInfo bookingInfo,
            BookingLimitType state,
            int addDaysForStartDate = 0,
            int addDaysForEndDate = 0)
        {
            return new AvailabilityStatusMessage
            {
                StartDate = bookingInfo.StartBooking.AddDays(addDaysForStartDate),
                EndDate = bookingInfo.EndBooking.AddDays(addDaysForEndDate),
                RoomId = bookingInfo.RoomId,
                State = state,
            };
        }
    }
}
