using CalendarIntegrationCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TLConnect;

namespace CalendarIntegrationWeb.Services
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
