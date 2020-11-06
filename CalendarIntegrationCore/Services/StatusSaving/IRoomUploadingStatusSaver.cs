using System;
using CalendarIntegrationCore.Models;

namespace CalendarIntegrationCore.Services.StatusSaving
{
    public interface IRoomUploadingStatusSaver: IObservable<RoomUploadStatus>
    {
        void SetRoomStatus(int roomId, string status, string message);
    }
}