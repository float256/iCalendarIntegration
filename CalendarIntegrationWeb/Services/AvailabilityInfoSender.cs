using CalendarIntegrationCore.Models;
using CalendarIntegrationCore.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;
using TLConnect;

namespace CalendarIntegrationWeb.Services
{
    public class AvailabilityInfoSender : IAvailabilityInfoSender
    {
        private readonly ISoapRequestCreator _soapRequestCreator;
        private readonly ILogger _logger;
        private readonly ITLConnectService _tlConnectService;

        public AvailabilityInfoSender(ISoapRequestCreator soapRequestCreator, ILogger<AvailabilityInfoSender> logger, ITLConnectService tlConnectService)
        {
            _soapRequestCreator = soapRequestCreator;
            _logger = logger;
            _tlConnectService = tlConnectService;
        }

        public async Task SendForOneHotel(BookingInfoChangesForHotel hotelChanges)
        {
            HotelAvailNotifRQRequest request = _soapRequestCreator.CreateRequest(hotelChanges);
            await _tlConnectService.HotelAvailNotifRQAsync(request);
        }
    }
}
