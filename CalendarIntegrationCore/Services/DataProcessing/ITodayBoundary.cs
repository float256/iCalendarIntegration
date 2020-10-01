using System;

namespace CalendarIntegrationCore.Services.DataProcessing
{
    public interface ITodayBoundary
    {
        bool IsFutureDate(DateTime date);
        bool IsWithinSpecifiedLimits(DateTime verifiableDate, DateTime startDate, DateTime endDate);
        bool IsWithinSpecifiedLimits(DateTime verifiableDate, DateTime endDate);
        DateTime GetBoundaryDate(int numberOfDaysFromToday);
    }
}