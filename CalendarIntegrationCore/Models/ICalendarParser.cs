using System;
using System.Collections.Generic;

namespace CalendarIntegrationCore.Models
{
    public interface ICalendarParser
    {
        List<BookingInfo> ParseCalendar(string calendar, int roomId);
        DateTime ParseDate(string dateInInitialFormat);
    }
}