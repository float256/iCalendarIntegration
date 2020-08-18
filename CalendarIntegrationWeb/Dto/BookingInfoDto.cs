using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CalendarIntegrationWeb.Dto
{
    public class BookingInfoDto
    {
        [Required()]
        public int Id { get; set; }

        [Required()]
        public int RoomId { get; set; }

        [Required()]
        public DateTime StartBooking { get; set; }

        [Required()]
        public DateTime EndBooking { get; set; }
    }
}
