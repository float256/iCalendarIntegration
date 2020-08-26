using CalendarIntegrationCore.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace CalendarIntegrationCore.Services
{
    public interface IAvailabilityInfoReceiver
    {
        string GetCalendarByUrl(string url, CancellationToken cancelToken);
    }
}
