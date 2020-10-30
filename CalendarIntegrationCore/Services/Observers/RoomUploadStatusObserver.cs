using System;
using System.Collections.Generic;
using CalendarIntegrationCore.Models;

namespace CalendarIntegrationCore.Services.Observers
{
    public class RoomUploadStatusObserver: IRoomUploadStatusObserver
    {
        public Dictionary<int, RoomUploadStatus> UploadStatusesForAllRooms { get; private set; }
        
        
        public RoomUploadStatusObserver()
        {
            UploadStatusesForAllRooms = new Dictionary<int, RoomUploadStatus>();
        }
        
        public void OnCompleted()
        {
            throw new NotImplementedException();
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        public void OnNext(RoomUploadStatus newRoomUploadStatus)
        {
            UploadStatusesForAllRooms[newRoomUploadStatus.Id] = newRoomUploadStatus;
        }
    }
}