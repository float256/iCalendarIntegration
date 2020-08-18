using CalendarIntegrationCore.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace CalendarIntegrationCore.Services.Repositories
{
    public interface IBookingInfoRepository
    {
        BookingInfo Get(int id);
        List<BookingInfo> GetByRoomId(int roomId);
        List<BookingInfo> GetAll();
        void Add(BookingInfo bookingInfo);
        void Update(BookingInfo bookingInfo);
        void Delete(int id);
    }
}
