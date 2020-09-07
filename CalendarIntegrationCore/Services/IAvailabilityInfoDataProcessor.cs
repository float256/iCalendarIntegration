using CalendarIntegrationCore.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace CalendarIntegrationCore.Services
{
    public interface IAvailabilityInfoDataProcessor
    {
        BookingInfoChanges GetChanges(List<BookingInfo> initialAvailabilityInfo, List<BookingInfo> newAvailabilityInfo);
        List<AvailabilityStatusMessage> FillGapsInDates(List<AvailabilityStatusMessage> bookingInfoForOccupiedRooms, string tlApiCode);
        List<AvailabilityStatusMessage> CreateAvailabilityStatusMessages(BookingInfoChanges infoChanges,
            string tlApiCode);
    }
}
