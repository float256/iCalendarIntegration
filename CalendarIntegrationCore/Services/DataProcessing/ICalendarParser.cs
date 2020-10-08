using System;
using System.Collections.Generic;
using CalendarIntegrationCore.Models;

namespace CalendarIntegrationCore.Services.DataProcessing
{
    public interface ICalendarParser
    {
        List<BookingInfo> ParseCalendar(string calendar, int roomId);
        DateTime ParseDate(string dateInInitialFormat);
    }
}