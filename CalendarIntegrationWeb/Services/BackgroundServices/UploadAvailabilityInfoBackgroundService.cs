using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CalendarIntegrationCore.Models;
using CalendarIntegrationCore.Services.DataRetrieving;
using CalendarIntegrationWeb.Services.DataUploading;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CalendarIntegrationWeb.Services.BackgroundServices
{
    public class UploadAvailabilityInfoBackgroundService : BackgroundService, IDisposable
    {
        private readonly TimeSpan _timerPeriod;
        private readonly int _dataPackageSize;
        private readonly ILogger<UploadAvailabilityInfoBackgroundService> _logger;
        private readonly IAvailabilityStatusMessageQueue _queue;
        private readonly IAvailabilityInfoSender _infoSender;
        private readonly IServiceProvider _serviceProvider;
        
        public UploadAvailabilityInfoBackgroundService(
            ILogger<UploadAvailabilityInfoBackgroundService> logger,
            IOptions<UploadAvailabilityInfoBackgroundServiceOptions> options,
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
            _logger.LogInformation("UploadAvailabilityInfoBackgroundService background is starting");
            while (!cancellationToken.IsCancellationRequested)
            {
                using (IServiceScope scope = _serviceProvider.CreateScope())
                {
                    List<AvailabilityStatusMessage> availMessages = _queue.PeekMultiple(_dataPackageSize);
                    try
                    {
                        await _infoSender.SendAvailabilityInfo(availMessages, cancellationToken);
                        _logger.LogInformation("Availability rooms information has been uploaded to TLConnect");
                        _queue.DequeueMultiple(_dataPackageSize);
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