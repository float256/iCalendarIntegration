using System;
using System.Collections.Generic;
using System.Linq;
using CalendarIntegrationCore.Models;
using CalendarIntegrationWeb.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace CalendarIntegrationWeb.Observers
{
    public class RoomUploadStatusObserver : IObserver<RoomUploadStatus>
    {
        private readonly IHubContext<RoomUploadStatusHub> _hub;

        public RoomUploadStatusObserver( IHubContext<RoomUploadStatusHub> hub )
        {
            _hub = hub;
        }

        public void OnCompleted()
        {
            throw new NotImplementedException();
        }

        public void OnError( Exception error )
        {
            throw new NotImplementedException();
        }

        public void OnNext( RoomUploadStatus newRoomUploadStatus )
        {
            _hub.Clients.All.SendAsync("transferRoomUploadStatus", newRoomUploadStatus);
        }
    }
}