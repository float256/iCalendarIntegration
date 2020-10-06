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
            return date > DateTime.Now;
        }

        public DateTime GetMaxDate()
        {
            return DateTime.Now.Add(TimeSpan.FromDays(_synchronizationDaysInFuture));
        }

        public DateTime GetMinDate()
        {
            return DateTime.Now;
        }
    }
}