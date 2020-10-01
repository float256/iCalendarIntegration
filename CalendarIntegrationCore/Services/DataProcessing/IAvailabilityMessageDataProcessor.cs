using System.Collections.Generic;
using CalendarIntegrationCore.Models;

namespace CalendarIntegrationCore.Services.DataProcessing
{
    public interface IAvailabilityMessageDataProcessor
    {
        List<AvailabilityStatusMessage> FillGapsInDates(
            List<AvailabilityStatusMessage> availabilityMessagesForOccupiedRooms,
            int roomId);

        List<AvailabilityStatusMessage> CreateAvailabilityStatusMessages(BookingInfoChanges infoChanges);
    }
}