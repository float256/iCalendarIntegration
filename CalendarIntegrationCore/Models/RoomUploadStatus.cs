using System.ComponentModel.DataAnnotations.Schema;

namespace CalendarIntegrationCore.Models
{
    public class RoomUploadStatus
    {
        [Column("id_room_upload_status")]
        public int Id { get; set; }

        [Column("id_room")]
        public int RoomId { get; set; }

        [Column("status")]
        public string Status { get; set; }

        [Column("message")]
        public string Message { get; set; }
    } 
}