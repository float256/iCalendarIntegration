using System;
using CalendarIntegrationCore.Services.DataSaving;
using Microsoft.Extensions.Options;

namespace CalendarIntegrationCore.Services.DataProcessing
{
    public class TodayBoundary: ITodayBoundary
    {
        private readonly int _synchronizationDaysInFuture;

        public TodayBoundary(IOptions<DateSynchronizationCommonOptions> options)
        {
            _synchronizationDaysInFuture = options.Value.SynchronizationDaysInFuture;
        }
        
        public bool IsFutureDate(DateTime date)
        {
            return date > GetMinDate();
        }

        public DateTime GetMaxDate()
        {
            return DateTime.UtcNow.Date.Add(TimeSpan.FromDays(_synchronizationDaysInFuture));
        }

        public DateTime GetMinDate()
        {
            return DateTime.UtcNow.Date.AddDays(-1);
        }
    }
}