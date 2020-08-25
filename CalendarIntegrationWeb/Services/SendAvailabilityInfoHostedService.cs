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
    public class SendAvailabilityInfoHostedService : BackgroundService, IDisposable
    {
        private readonly TimeSpan _timerPeriod;
        private readonly ILogger<SendAvailabilityInfoHostedService> _logger;
        private readonly IAvailabilityInfoService _availabilityInfoService;
        private Timer _timer;

        public SendAvailabilityInfoHostedService(
            ILogger<SendAvailabilityInfoHostedService> logger,
            IOptions<SendAvailabilityInfoHostedServiceOptions> options,
            IAvailabilityInfoService availabilityInfoService)
        {
            _logger = logger;
            _availabilityInfoService = availabilityInfoService;
            _timerPeriod = TimeSpan.FromSeconds(options.Value.SendingPeriodInSeconds);
        }

        private void RunTask(object state)
        {
            _availabilityInfoService.ProcessAllInfo((CancellationToken) state);
            _logger.LogInformation("Availability rooms information has been sent");
        }

        protected override Task ExecuteAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(RunTask, cancellationToken, TimeSpan.Zero, _timerPeriod);
            _logger.LogInformation("SendAvailabilityInfoHostedService background is starting");
            return Task.CompletedTask;
        }
        public override void Dispose()
        {
            _timer?.Dispose();
            base.Dispose();
        }
    }
}