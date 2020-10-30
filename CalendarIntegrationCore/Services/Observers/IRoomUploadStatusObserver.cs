using System;
using System.Collections.Generic;
using CalendarIntegrationCore.Models;

namespace CalendarIntegrationCore.Services.Observers
{
    public interface IRoomUploadStatusObserver: IObserver<RoomUploadStatus>
    {
        Dictionary<int, RoomUploadStatus> UploadStatusesForAllRooms { get; }
    }
}