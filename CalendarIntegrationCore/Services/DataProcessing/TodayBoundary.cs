using System;

namespace CalendarIntegrationCore.Services.DataProcessing
{
    public class TodayBoundary: ITodayBoundary
    {
        public bool IsFutureDate(DateTime date)
        {
            return date > DateTime.Now;
        }

        public bool IsWithinSpecifiedLimits(DateTime verifiableDate, DateTime startDate, DateTime endDate)
        {
            return (verifiableDate > startDate) && (verifiableDate < endDate);
        }

        public bool IsWithinSpecifiedLimits(DateTime verifiableDate, DateTime endDate)
        {
            return IsWithinSpecifiedLimits(verifiableDate, DateTime.Now, endDate);
        }
        
        public DateTime GetBoundaryDate(int numberOfDaysFromToday)
        {
            return DateTime.Now.Add(TimeSpan.FromDays(numberOfDaysFromToday));
        }
    }
}