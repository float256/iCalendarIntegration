using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace CalendarIntegrationCore.Models
{
    public class AvailabilityStatusMessage
    {
        [Column("id_availability_status_message")]
        public int Id { get; set; }
        
        [Column("id_room")]
        public int RoomId { get; set; }
        
        [Column("start_date")]
        public DateTime StartDate { get; set; }
        
        [Column("end_date")]
        public DateTime EndDate { get; set; }

        [Column("state")]
        public BookingLimitType State { get; set; }
    }
}