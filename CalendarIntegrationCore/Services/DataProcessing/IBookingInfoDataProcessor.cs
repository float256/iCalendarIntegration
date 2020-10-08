using System.Collections.Generic;
using CalendarIntegrationCore.Models;

namespace CalendarIntegrationCore.Services.DataProcessing
{
    public interface IBookingInfoDataProcessor
    {
        BookingInfoChanges GetChanges(List<BookingInfo> newBookingInfos, List<BookingInfo> initialBookingInfos);
    }
}
