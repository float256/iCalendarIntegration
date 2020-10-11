using System;
using System.Threading;
using System.Threading.Tasks;
using CalendarIntegrationCore.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CalendarIntegrationWeb.Services.BackgroundServices
{
    public class DownloadAvailabilityInfoBackgroundService : BackgroundService, IDisposable
    {
        private readonly TimeSpan _timerPeriod;
        private readonly ILogger<DownloadAvailabilityInfoBackgroundService> _logger;
        private readonly IServiceProvider _serviceProvider;

        public DownloadAvailabilityInfoBackgroundService(
            ILogger<DownloadAvailabilityInfoBackgroundService> logger,
            IOptions<DownloadAvailabilityInfoBackgroundServiceOptions> options,
            IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _timerPeriod = TimeSpan.FromSeconds(options.Value.DownloadPeriodInSeconds);
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("DownloadAvailabilityInfoBackgroundService background is starting");
            while (!cancellationToken.IsCancellationRequested)
            {
                using (IServiceScope scope = _serviceProvider.CreateScope())
                {
                    IAvailabilityInfoSynchronizer availabilitySynchronizer = scope.ServiceProvider.GetRequiredService<IAvailabilityInfoSynchronizer>();
                    await availabilitySynchronizer.ProcessAllInfo(cancellationToken);
                }
                _logger.LogInformation("Availability rooms information has been downloaded");
                await Task.Delay(_timerPeriod, cancellationToken);
            }
        }
    }
}