using System.ComponentModel.DataAnnotations.Schema;

namespace CalendarIntegrationCore.Models
{
    public class RoomUploadStatus
    {
        public int RoomId { get; set; }
        
        public string Status { get; set; }
        
        public string Message { get; set; }
    }
}