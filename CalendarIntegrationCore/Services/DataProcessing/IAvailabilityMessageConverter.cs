using System;
using System.Collections.Generic;
using CalendarIntegrationCore.Models;

namespace CalendarIntegrationCore.Services.DataProcessing
{
    public interface IAvailabilityMessageConverter
    {
        List<AvailabilityStatusMessage> CreateAvailabilityStatusMessages(BookingInfoChanges infoChanges);

        AvailabilityStatusMessage CreateAvailabilityStatusMessage(
            BookingInfo bookingInfo,
            BookingLimitType state,
            int addDaysForStartDate = 0,
            int addDaysForEndDate = 0);
    }
}