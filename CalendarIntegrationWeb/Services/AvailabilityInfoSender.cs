using CalendarIntegrationCore.Models;
using CalendarIntegrationCore.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;
using CalendarIntegrationCore.Services.Repositories;
using CalendarIntegrationWeb.Dto;
using Microsoft.EntityFrameworkCore.Internal;
using TLConnect;

namespace CalendarIntegrationWeb.Services
{
    public class AvailabilityInfoSender : IAvailabilityInfoSender
    {
        private readonly ISoapRequestCreator _soapRequestCreator;
        private readonly IHotelRepository _hotelRepository;
        private readonly IRoomRepository _roomRepository;
        private readonly ILogger _logger;
        private readonly ITLConnectService _tlConnectService;

        public AvailabilityInfoSender(
            ISoapRequestCreator soapRequestCreator,
            ILogger<AvailabilityInfoSender> logger, 
            IHotelRepository hotelRepository,
            IRoomRepository roomRepository,
            ITLConnectService tlConnectService)
        {
            _soapRequestCreator = soapRequestCreator;
            _logger = logger;
            _hotelRepository = hotelRepository;
            _roomRepository = roomRepository;
            _tlConnectService = tlConnectService;
        }

        public async Task SendAvailabilityInfo(
            List<AvailabilityStatusMessage> availStatuses,
            CancellationToken cancellationToken)
        {
            if (!cancellationToken.IsCancellationRequested)
            {
                var availStatusesWithHotelInfo = (from availStatus in availStatuses
                    join room in _roomRepository.GetAll() on availStatus.RoomId equals room.Id
                    join hotel in _hotelRepository.GetAll() on room.HotelId equals hotel.Id
                    group availStatus by hotel);
                foreach (var availStatusGroup in availStatusesWithHotelInfo)
                {
                    Hotel currHotel = availStatusGroup.Key;
                    List<AvailabilityStatusMessage> availStatusMessagesForCurrHotel = availStatusGroup.ToList();
                    var request = _soapRequestCreator.CreateRequest(availStatusMessagesForCurrHotel, currHotel.Login,
                        currHotel.Password,
                        currHotel.HotelCode);
                    await _tlConnectService.HotelAvailNotifRQAsync(request);
                }
            }
        }
    }
}
