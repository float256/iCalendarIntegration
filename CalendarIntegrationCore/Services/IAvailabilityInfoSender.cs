using CalendarIntegrationCore.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CalendarIntegrationCore.Services
{
    public interface IAvailabilityInfoSender
    {
        Task SendAvailabilityInfo(List<AvailabilityStatusMessage> availStatuses,
            CancellationToken cancellationToken);
    }
}
