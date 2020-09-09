using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CalendarIntegrationCore.Models;

namespace CalendarIntegrationCore.Services.DataUploading
{
    public interface IAvailabilityInfoSender
    {
        Task SendAvailabilityInfo(List<AvailabilityStatusMessage> availStatuses,
            CancellationToken cancellationToken);
    }
}
