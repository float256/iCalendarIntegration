﻿using CalendarIntegrationCore.Models;
using CalendarIntegrationCore.Services.Repositories;

namespace CalendarIntegrationCore.Services.DataSaving
{
    public class BookingInfoSaver: IBookingInfoSaver
    {
        private readonly IBookingInfoRepository _bookingInfoRepository;
        
        public BookingInfoSaver(IBookingInfoRepository bookingInfoRepository)
        {
            _bookingInfoRepository = bookingInfoRepository;
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
    }
}
