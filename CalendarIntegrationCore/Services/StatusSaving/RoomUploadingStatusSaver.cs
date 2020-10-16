using CalendarIntegrationCore.Models;
using CalendarIntegrationCore.Services.Repositories;

namespace CalendarIntegrationCore.Services.StatusSaving
{
    public class RoomUploadingStatusSaver: IRoomUploadingStatusSaver
    {
        private readonly IRoomUploadStatusRepository _roomUploadStatusRepository;

        public RoomUploadingStatusSaver(IRoomUploadStatusRepository roomUploadStatusRepository)
        {
            _roomUploadStatusRepository = roomUploadStatusRepository;
        }
        
        public void SetRoomStatus(int roomId, string status, string message)
        {
            _roomUploadStatusRepository.SetStatus(new RoomUploadStatus
            {
                RoomId = roomId,
                Status = status,
                Message = message
            });
        }
    }
}