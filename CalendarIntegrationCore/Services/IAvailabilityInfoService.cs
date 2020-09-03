﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CalendarIntegrationCore.Services
{
    public interface IAvailabilityInfoService
    {
        Task ProcessAllInfo(CancellationToken cancelToken);
    }
}
