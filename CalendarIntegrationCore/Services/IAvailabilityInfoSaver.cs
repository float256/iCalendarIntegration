using CalendarIntegrationCore.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace CalendarIntegrationCore.Services
{
    public interface IAvailabilityInfoSaver
    {
        void SaveChanges(BookingInfoChanges changes);
        void AddAllBookingInfoForRoomInQueue(Room room, bool isFillGaps = false, int upperBoundForLoadedDatesInDays = 730);
    }
}
