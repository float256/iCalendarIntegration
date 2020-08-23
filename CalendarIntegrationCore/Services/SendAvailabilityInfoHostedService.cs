using System;
using System.Threading;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace CalendarIntegrationCore.Services
{
    public class SendAvailabilityInfoHostedService : IDisposable, IHostedService
    {
        private readonly int _taskPeriod;
        private readonly IAvailabilityInfoSender _infoSender;
        private readonly ILogger<SendAvailabilityInfoHostedService> _logger;
        private Timer _timer;


        public SendAvailabilityInfoHostedService(IAvailabilityInfoSender infoSender, int taskPeriod, 
            ILogger<SendAvailabilityInfoHostedService> logger)
        {
            _infoSender = infoSender;
            _taskPeriod = taskPeriod;
            _logger = logger;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }

        public void RunTask(object state)
        {
            _infoSender.SaveAndSendAllInfo();
        }
        
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("SendAvailabilityInfoHostedService is started");
            _timer = new Timer(RunTask, null, TimeSpan.Zero, 
                TimeSpan.FromSeconds(_taskPeriod));
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