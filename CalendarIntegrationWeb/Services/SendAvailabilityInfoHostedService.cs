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
    public class SendAvailabilityInfoHostedService : IDisposable, IHostedService
    {
        private readonly TimeSpan _timerPeriod;
        private readonly IAvailabilityInfoSender _infoSender;
        private readonly ILogger<SendAvailabilityInfoHostedService> _logger;
        private Timer _timer;

        public SendAvailabilityInfoHostedService(
            IAvailabilityInfoSender infoSender, 
            ILogger<SendAvailabilityInfoHostedService> logger,
            IOptions<SendAvailabilityInfoHostedServiceOptions> options)
        {
            _infoSender = infoSender;
            _logger = logger;
            _timerPeriod = TimeSpan.FromSeconds(options.Value.SendingPeriodInSeconds);
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }

        public void RunTask(object state)
        {
            _infoSender.SaveAndSendAllInfo();
            _logger.LogInformation("Availability rooms information has been sent");
        }
        
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("SendAvailabilityInfoHostedService is started");
            _timer = new Timer(RunTask, null, TimeSpan.Zero, _timerPeriod);
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("SendAvailabilityInfoHostedService is stopping");
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }
    }
}