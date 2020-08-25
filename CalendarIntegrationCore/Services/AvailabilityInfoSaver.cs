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

        public AvailabilityInfoSaver(IBookingInfoRepository bookingInfoRepository)
        {
            _bookingInfoRepository = bookingInfoRepository;
        }

        public void SaveChanges(BookingInfoChanges changes)
        {
            foreach (BookingInfo bookingInfo in changes.AddedBookingInfo)
            {
                _bookingInfoRepository.Add(bookingInfo);
            }

            foreach (BookingInfo bookingInfo in changes.RemovedBookingInfo)
            {
                _bookingInfoRepository.Delete(bookingInfo.Id);
            }
        }
    }
}
