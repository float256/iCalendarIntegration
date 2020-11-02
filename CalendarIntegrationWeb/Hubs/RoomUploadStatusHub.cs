using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CalendarIntegrationCore.Models;
using CalendarIntegrationWeb.Observers;
using Microsoft.AspNetCore.SignalR;

namespace CalendarIntegrationWeb.Hubs
{
    public class RoomUploadStatusHub : Hub
    {
        private readonly IRoomUploadStatusObserver _roomUploadStatusObserver;
        
        public RoomUploadStatusHub(IRoomUploadStatusObserver roomUploadStatusObserver)
        {
            _roomUploadStatusObserver = roomUploadStatusObserver;
        }
        
        public override async Task OnConnectedAsync()
        {
            await Clients.All.SendAsync( 
                "transferRoomUploadStatus", 
                _roomUploadStatusObserver.UploadStatusesForAllRooms.Values.ToList());
        }
    }
}