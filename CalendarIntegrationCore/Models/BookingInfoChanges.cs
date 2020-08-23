using System.Collections.Generic;

namespace CalendarIntegrationCore.Models
{
    public class BookingInfoChanges
    {
        public List<BookingInfo> AddedBookingInfo { get; set; } = new List<BookingInfo>();
        public List<BookingInfo> RemovedBookingInfo { get; set; } = new List<BookingInfo>();
    }
}