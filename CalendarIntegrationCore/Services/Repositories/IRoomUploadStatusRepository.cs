using CalendarIntegrationCore.Models;

namespace CalendarIntegrationCore.Services.Repositories
{
    public interface IRoomUploadStatusRepository
    {

        RoomUploadStatus Get(int id);
        RoomUploadStatus GetByRoomId(int roomId);
        void SetStatus(RoomUploadStatus newStatus);
        void DeleteByRoomId(int roomId);
    }
}
