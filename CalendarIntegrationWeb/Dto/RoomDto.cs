using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CalendarIntegrationWeb.Dto
{
    public class RoomDto
    {
        [Required()]
        public int Id { get; set; }

        [Required()]
        public int HotelId { get; set; }

        [Required()]
        public string TLApiCode { get; set; }

        [Required()]
        public string Url { get; set; }

        [Required()]
        public string Name { get; set; }
        
        public string Status { get; set; }
        public string StatusMessage { get; set; }
    }
}
