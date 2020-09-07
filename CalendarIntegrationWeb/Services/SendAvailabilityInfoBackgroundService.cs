using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CalendarIntegrationCore.Models;
using CalendarIntegrationCore.Services;
using CalendarIntegrationCore.Services.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CalendarIntegrationWeb.Services
{
    public class SendAvailabilityInfoBackgroundService : BackgroundService, IDisposable
    {
        private readonly TimeSpan _timerPeriod;
        private readonly int _dataPackageSize;
        private readonly ILogger<SendAvailabilityInfoBackgroundService> _logger;
        private readonly IAvailabilityStatusMessageQueue _queue;
        private readonly IAvailabilityInfoSender _infoSender;
        private readonly IServiceProvider _serviceProvider;
        
        public SendAvailabilityInfoBackgroundService(
            ILogger<SendAvailabilityInfoBackgroundService> logger,
            IOptions<SendAvailabilityInfoBackgroundServiceOptions> options,
            IAvailabilityStatusMessageQueue queue,
            IAvailabilityInfoSender infoSender,
            IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _timerPeriod = TimeSpan.FromSeconds(options.Value.SendingPeriodInSeconds);
            _dataPackageSize = options.Value.DataPackageSize;
            _queue = queue;
            _infoSender = infoSender;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("SendAvailabilityInfoHostedService background is starting");
            while (!cancellationToken.IsCancellationRequested)
            {
                using (IServiceScope scope = _serviceProvider.CreateScope())
                {
                    List<AvailabilityStatusMessage> availMessages = _queue.DequeueMultiple(
                        Math.Min(_dataPackageSize, _queue.Count));
                    try
                    {
                        await _infoSender.SendAvailabilityInfo(availMessages, cancellationToken);
                        _logger.LogInformation("Availability rooms information has been sent");
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e, "Error occurred while trying to send data to TLConnect");
                    }
                }
                await Task.Delay(_timerPeriod, cancellationToken);
            }
        }
    }
}