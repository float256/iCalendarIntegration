using System;
using System.Threading;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using CalendarIntegrationCore.Services;
using Microsoft.Extensions.Options;

namespace CalendarIntegrationWeb.Services
{
    public class SaveAvailabilityInfoBackgroundService : BackgroundService, IDisposable
    {
        private readonly TimeSpan _timerPeriod;
        private readonly ILogger<SaveAvailabilityInfoBackgroundService> _logger;
        private readonly IServiceProvider _serviceProvider;

        public SaveAvailabilityInfoBackgroundService(
            ILogger<SaveAvailabilityInfoBackgroundService> logger,
            IOptions<SaveAvailabilityInfoBackgroundServiceOptions> options,
            IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _timerPeriod = TimeSpan.FromSeconds(options.Value.SavingPeriodInSeconds);
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("SaveAvailabilityInfoHostedService background is starting");
            while (!cancellationToken.IsCancellationRequested)
            {
                using (IServiceScope scope = _serviceProvider.CreateScope())
                {
                    IAvailabilityInfoService availabilityService = scope.ServiceProvider.GetRequiredService<IAvailabilityInfoService>();
                    availabilityService.ProcessAllInfo(cancellationToken);
                }
                _logger.LogInformation("Availability rooms information has been saved");
                await Task.Delay(_timerPeriod, cancellationToken);
            }
        }
    }
}