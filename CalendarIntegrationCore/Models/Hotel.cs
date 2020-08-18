using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CalendarIntegrationCore.Models
{
    public class Hotel
    {
        [Column("id_hotel")]
        public int Id { get; set; }

        [Column("hotel_code")]
        public string HotelCode { get; set; }

        [Column("login")]
        public string Login { get; set; }

        [Column("password")]
        public string Password { get; set; }

        [Column("name")]
        public string Name { get; set; }
    }
}
