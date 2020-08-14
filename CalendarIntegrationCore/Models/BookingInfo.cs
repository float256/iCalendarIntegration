using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CalendarIntegrationCore.Models
{
    public class BookingInfo
    {
        [Column("id_booking_info")]
        public int Id { get; set; }
        [Column("id_room")]
        public int RoomId { get; set; }
        [Column("start_booking")]
        public DateTime StartBooking { get; set; }
        [Column("end_booking")]
        public DateTime EndBooking { get; set; }
    }
}
