using System.Collections.Generic;
using System.Linq;
using CalendarIntegrationCore.Models;
using CalendarIntegrationCore.Services.InitializationHandlers;
using CalendarIntegrationCore.Services.Repositories;
using CalendarIntegrationCore.Services.StatusSaving;
using CalendarIntegrationWeb.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CalendarIntegrationWeb.Controllers
{
    [Route("api/Room")]
    [ApiController]
    public class RoomController : ControllerBase
    {
        private readonly IRoomRepository _roomRepository;
        private readonly IHotelRepository _hotelRepository;
        private readonly IRoomAvailabilityInitializationHandler _roomAvailabilityInitializationHandler;
        private readonly IRoomUploadingStatusSaver _roomUploadingStatusSaver;
        private readonly ILogger _logger;
        
        public RoomController(
            IRoomRepository roomRepository,
            IHotelRepository hotelRepository,
            IRoomAvailabilityInitializationHandler roomAvailabilityInitializationHandler,
            IRoomUploadingStatusSaver roomUploadingStatusSaver,
            ILogger<RoomController> logger)
        {
            _roomRepository = roomRepository;
            _hotelRepository = hotelRepository;
            _roomAvailabilityInitializationHandler = roomAvailabilityInitializationHandler;
            _roomUploadingStatusSaver = roomUploadingStatusSaver;
            _logger = logger;
        }

        [HttpGet("{id:int}")]
        public ActionResult<RoomDto> Get(int id)
        {
            Room room = _roomRepository.Get(id);
            if (room != default)
            {
                return Ok(new RoomDto
                {
                    Id = room.Id,
                    HotelId = room.HotelId,
                    Name = room.Name,
                    TLApiCode = room.TLApiCode,
                    Url = room.Url
                });
            }
            else
            {
                return NotFound();
            }
        }

        [HttpGet("GetByHotelId/{hotelId:int}")]
        public ActionResult<List<Room>> GetByHotelId(int hotelId)
        {
            List<RoomDto> result = _roomRepository.GetByHotelId(hotelId).Select(
                room => new RoomDto
                {
                    Id = room.Id,
                    HotelId = room.HotelId,
                    Name = room.Name,
                    TLApiCode = room.TLApiCode,
                    Url = room.Url
                }).ToList();
            return Ok(result);
        }

        [HttpGet("GetAll")]
        public ActionResult<List<RoomDto>> GetAll()
        {
            return _roomRepository.GetAll().Select(
                room => new RoomDto
                {
                    Id = room.Id,
                    HotelId = room.HotelId,
                    Name = room.Name,
                    TLApiCode = room.TLApiCode,
                    Url = room.Url
                }).ToList();
        }

        [HttpPost("Add")]
        public ActionResult<RoomDto> Add(RoomDto roomDto)
        {
            if (_hotelRepository.Get(roomDto.HotelId) != default)
            {
                Room room = new Room
                {
                    HotelId = roomDto.HotelId,
                    Name = roomDto.Name,
                    TLApiCode = roomDto.TLApiCode,
                    Url = roomDto.Url
                };
                _roomRepository.Add(room);
                try
                {
                    _roomAvailabilityInitializationHandler.AddAvailabilityMessagesForRoomToQueue(room);
                }
                catch (RoomAvailabilityInitializationHandlerException exception)
                {
                    _roomUploadingStatusSaver.SetRoomStatus(room.Id, "Add Availability Message Error", exception.Message);
                    _logger.LogError(exception, "Error occurred while trying to initialize room availability");                    
                }
                return Ok(room);
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpPost("Update")]
        public void Update(RoomDto roomDto)
        {
            Room previousRoom = _roomRepository.Get(roomDto.Id);
            _roomRepository.Detach(previousRoom);
            Room newRoom = new Room
            {
                Id = roomDto.Id,
                HotelId = roomDto.HotelId,
                Name = roomDto.Name,
                TLApiCode = roomDto.TLApiCode,
                Url = roomDto.Url
            };
            try
            {
                if (previousRoom.TLApiCode != roomDto.TLApiCode)
                {
                    _roomAvailabilityInitializationHandler.AddAvailabilityMessagesForRoomToQueue(newRoom);
                }
                _roomRepository.Update(newRoom);
            }
            catch (RoomAvailabilityInitializationHandlerException exception)
            {
                _roomUploadingStatusSaver.SetRoomStatus(newRoom.Id, "Add Availability Message Error", exception.Message);
                _logger.LogError(exception, "Error occurred while trying to adding availability messages");
            }
        }

        [HttpPost("Delete")]
        public void Delete([FromBody] int id)
        {
            _roomRepository.Delete(id);
        }
    }
}
