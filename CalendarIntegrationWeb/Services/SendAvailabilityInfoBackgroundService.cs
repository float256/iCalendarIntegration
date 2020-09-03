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
    public class SendAvailabilityInfoBackgroundService : BackgroundService, IDisposable
    {
        private readonly TimeSpan _timerPeriod;
        private readonly ILogger<SendAvailabilityInfoBackgroundService> _logger;
        private readonly IServiceProvider _serviceProvider;

        public SendAvailabilityInfoBackgroundService(
            ILogger<SendAvailabilityInfoBackgroundService> logger,
            IOptions<SendAvailabilityInfoBackgroundServiceOptions> options,
            IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _timerPeriod = TimeSpan.FromSeconds(options.Value.SendingPeriodInSeconds);
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
           
            _logger.LogInformation("SendAvailabilityInfoHostedService background is starting");
            while (!cancellationToken.IsCancellationRequested)
            {
                using (IServiceScope scope = _serviceProvider.CreateScope())
                {
                    IAvailabilityInfoService availabilityService = scope.ServiceProvider.GetRequiredService<IAvailabilityInfoService>();
                    await availabilityService.ProcessAllInfo(cancellationToken);
                }
                _logger.LogInformation("Availability rooms information has been sent");
                await Task.Delay(_timerPeriod, cancellationToken);
            }
        }
    }
}