using System.Collections.Generic;
using CalendarIntegrationCore.Models;

namespace CalendarIntegrationCore.Services.DataProcessing
{
    public interface IAvailabilityInfoDataProcessor
    {
        BookingInfoChanges GetChanges(List<BookingInfo> initialAvailabilityInfo, List<BookingInfo> newAvailabilityInfo);
        List<AvailabilityStatusMessage> FillGapsInDates(
            List<AvailabilityStatusMessage> bookingInfoForOccupiedRooms,
            int roomId);
        List<AvailabilityStatusMessage> CreateAvailabilityStatusMessages(BookingInfoChanges infoChanges,
            string tlApiCode);
    }
}
