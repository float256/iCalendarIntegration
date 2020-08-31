using System;
using System.Collections.Generic;
using System.Text;

namespace CalendarIntegrationCore.Models
{
    public class BookingInfoChangesForHotel
    {
        public List<BookingInfoChangesForRoom> ChangesForRooms { get; set; } = new List<BookingInfoChangesForRoom>();
        public Hotel Hotel { get; set; }
    }
}
