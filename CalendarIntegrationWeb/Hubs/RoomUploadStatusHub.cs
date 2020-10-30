﻿using System;
 using System.Collections.Generic;
 using System.Threading.Tasks;
using CalendarIntegrationCore.Models;
 using CalendarIntegrationCore.Services;
 using CalendarIntegrationCore.Services.Observers;
 using CalendarIntegrationCore.Services.Repositories;
using Microsoft.AspNetCore.SignalR;

namespace CalendarIntegrationWeb.Hubs
{
    public class RoomUploadStatusHub: Hub
    {
        private readonly IRoomUploadStatusObserver _roomUploadStatusObserver;
        
        public RoomUploadStatusHub(IRoomUploadStatusObserver roomUploadStatusObserver)
        {
            _roomUploadStatusObserver = roomUploadStatusObserver;
        }
        
        public async Task Send()
        {
            await Clients.All.SendAsync("RoomUploadStatus", _roomUploadStatusObserver.UploadStatusesForAllRooms);
        }
    }
}