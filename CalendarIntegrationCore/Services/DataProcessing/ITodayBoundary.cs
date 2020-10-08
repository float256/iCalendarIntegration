using System;

namespace CalendarIntegrationCore.Services.DataProcessing
{
    public interface ITodayBoundary
    {
        bool IsFutureDate(DateTime date);
        DateTime GetMinDate();
        DateTime GetMaxDate();
    }
}