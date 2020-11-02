using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CalendarIntegrationCore.Models;
using CalendarIntegrationCore.Services.DataProcessing;
using CalendarIntegrationCore.Services.DataSaving;
using CalendarIntegrationWeb.Services.DataUploading;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CalendarIntegrationWeb.Services.BackgroundServices
{
    public class UploadAvailabilityInfoBackgroundService : BackgroundService
    {
        private readonly TimeSpan _timerPeriod;
        private readonly int _dataPackageSize;
        private readonly ILogger<UploadAvailabilityInfoBackgroundService> _logger;
        private readonly IServiceProvider _serviceProvider;
        
        public UploadAvailabilityInfoBackgroundService(
            ILogger<UploadAvailabilityInfoBackgroundService> logger,
            IOptions<UploadAvailabilityInfoBackgroundServiceOptions> options,
            IAvailabilityInfoSender infoSender,
            IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _timerPeriod = TimeSpan.FromSeconds(options.Value.SendingPeriodInSeconds);
            _dataPackageSize = options.Value.DataPackageSize;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("UploadAvailabilityInfoBackgroundService background is starting");
            while (!cancellationToken.IsCancellationRequested)
            {
                using (IServiceScope scope = _serviceProvider.CreateScope())
                {
                    IAvailabilityStatusMessageQueue queue = scope.ServiceProvider
                        .GetRequiredService<IAvailabilityStatusMessageQueue>();
                    ITodayBoundary todayBoundary = scope.ServiceProvider
                        .GetRequiredService<ITodayBoundary>();
                    IAvailabilityInfoSender infoSender = scope.ServiceProvider.
                        GetRequiredService<IAvailabilityInfoSender>();
                    
                    List<AvailabilityStatusMessage> availMessages = queue.PeekMultiple(_dataPackageSize).Select(
                        availMessage =>
                        {
                            if (availMessage.StartDate < todayBoundary.GetMinDate())
                            {
                                availMessage.StartDate = todayBoundary.GetMinDate();
                            }
                            return availMessage;
                        }).ToList();
                    try
                    {
                        await infoSender.SendAvailabilityInfo(availMessages, cancellationToken);
                        _logger.LogInformation("Availability rooms information has been uploaded to TLConnect");
                        queue.DequeueMultiple(_dataPackageSize);
                    }
                    catch (Exception exception)
                    {
                        _logger.LogError(exception, "Error occurred while trying to send data to TLConnect");
                    }
                }
                await Task.Delay(_timerPeriod, cancellationToken);
            }
            _logger.LogInformation("UploadAvailabilityInfoBackgroundService background is stopped");
        }
    }
}