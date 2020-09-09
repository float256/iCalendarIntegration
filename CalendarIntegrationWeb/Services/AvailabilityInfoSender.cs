using CalendarIntegrationCore.Models;
using CalendarIntegrationCore.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;
using CalendarIntegrationCore.Services.DataUploading;
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
                List<Room> roomsForAvailStatuses = _roomRepository.GetMultiple(
                    availStatuses.Select(elem => elem.RoomId).ToList());
                List<Hotel> hotelsForRooms = _hotelRepository.GetMultiple(
                    roomsForAvailStatuses.Select(elem => elem.HotelId).ToList());
                List<IGrouping<Room, AvailabilityStatusMessage>> availStatusesGroupedByRooms = (
                    from availStatus in availStatuses
                    join room in roomsForAvailStatuses on availStatus.RoomId equals room.Id
                    group availStatus by room).ToList();
                List<IGrouping<Hotel, IGrouping<Room, AvailabilityStatusMessage>>> availStatusesGroupedByRoomsAndHotels = (
                    from groupedAvailStatuses in availStatusesGroupedByRooms
                    join hotel in hotelsForRooms on groupedAvailStatuses.Key.HotelId equals hotel.Id
                    group groupedAvailStatuses by hotel).ToList();
                foreach (var roomGroup in availStatusesGroupedByRoomsAndHotels)
                {
                    Hotel currHotel = roomGroup.Key;
                    Dictionary<Room, List<AvailabilityStatusMessage>> availStatusesDict = new Dictionary<Room, 
                        List<AvailabilityStatusMessage>>();
                    foreach (var currRoomGroup in roomGroup)
                    {
                        availStatusesDict.Add(currRoomGroup.Key, currRoomGroup.ToList());
                    }
                    HotelAvailNotifRQRequest request = _soapRequestCreator.CreateRequest(
                        availStatusesDict, 
                        currHotel.Login,
                        currHotel.Password,
                        currHotel.HotelCode);
                    await _tlConnectService.HotelAvailNotifRQAsync(request);
                }
            }
        }
    }
}
