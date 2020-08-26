using CalendarIntegrationCore.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace CalendarIntegrationCore.Services
{
    public interface IAvailabilityInfoDataProcessor
    {
        BookingInfoChanges GetChanges(List<BookingInfo> initialAvailabilityInfo, List<BookingInfo> newAvailabilityInfo);
    }
}
