﻿using System.Collections.Generic;
using CalendarIntegrationCore.Models;
using TLConnect;

namespace CalendarIntegrationWeb.Services.DataUploading
{
    public interface ISoapRequestCreator
    {
        HotelAvailNotifRQRequest CreateRequest(
            Dictionary<Room, List<AvailabilityStatusMessage>> availStatuses,
            string username,
            string password,
            string hotelCode);
    }
}
