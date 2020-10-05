using System.Collections.Generic;
using CalendarIntegrationCore.Models;

namespace CalendarIntegrationCore.Services.DataProcessing
{
    public interface IAvailabilityMessageConverter
    {
        List<AvailabilityStatusMessage> CreateAvailabilityStatusMessages(BookingInfoChanges infoChanges);
    }
}