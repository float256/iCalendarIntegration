using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace CalendarIntegrationCore.Services
{
    public interface IAvailabilityInfoService
    {
        void ProcessAllInfo(CancellationToken cancelToken);
    }
}
