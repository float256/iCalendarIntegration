using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CalendarIntegrationCore.Models;
using CalendarIntegrationCore.Services.DataProcessing;
using CalendarIntegrationCore.Services.DataRetrieving;
using CalendarIntegrationCore.Services.DataSaving;
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
        private readonly ITodayBoundary _todayBoundary;
        
        public UploadAvailabilityInfoBackgroundService(
            ILogger<UploadAvailabilityInfoBackgroundService> logger,
            IOptions<UploadAvailabilityInfoBackgroundServiceOptions> options,
            IAvailabilityStatusMessageQueue queue,
            IAvailabilityInfoSender infoSender,
            ITodayBoundary todayBoundary,
            IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _timerPeriod = TimeSpan.FromSeconds(options.Value.SendingPeriodInSeconds);
            _dataPackageSize = options.Value.DataPackageSize;
            _queue = queue;
            _infoSender = infoSender;
            _todayBoundary = todayBoundary;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("UploadAvailabilityInfoBackgroundService background is starting");
            while (!cancellationToken.IsCancellationRequested)
            {
                using (IServiceScope scope = _serviceProvider.CreateScope())
                {
                    List<AvailabilityStatusMessage> availMessages = _queue.PeekMultiple(_dataPackageSize).Select(
                        availMessage =>
                        {
                            if (availMessage.StartDate < _todayBoundary.GetMinDate())
                            {
                                availMessage.StartDate = _todayBoundary.GetMinDate();
                            }
                            return availMessage;
                        }).ToList();
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