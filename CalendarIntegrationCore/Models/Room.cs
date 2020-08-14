using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CalendarIntegrationCore.Models
{
    public class Room
    {
        [Column("id_room")]
        public int Id { get; set; }
        [Column("id_hotel")]
        public int HotelId { get; set; }
        [Column("tl_api_code")]
        public string TLApiCode { get; set; }
        [Column("url")]
        public string Url { get; set; }
        [Column("name")]
        public string Name { get; set; }
    }
}
