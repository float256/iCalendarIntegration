using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CalendarIntegrationCore.Models;
using CalendarIntegrationCore.Services;
using CalendarIntegrationCore.Services.DataSaving;
using CalendarIntegrationCore.Services.InitializationHandlers;
using CalendarIntegrationCore.Services.Repositories;
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
        private readonly IRoomUploadStatusRepository _roomUploadStatusRepository;
        private readonly ILogger _logger;
        
        public RoomController(
            IRoomRepository roomRepository,
            IHotelRepository hotelRepository,
            IRoomAvailabilityInitializationHandler roomAvailabilityInitializationHandler,
            IRoomUploadStatusRepository roomUploadStatusRepository,
            ILogger<RoomController> logger)
        {
            _roomRepository = roomRepository;
            _hotelRepository = hotelRepository;
            _roomAvailabilityInitializationHandler = roomAvailabilityInitializationHandler;
            _roomUploadStatusRepository = roomUploadStatusRepository;
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
                    _roomUploadStatusRepository.SetStatus(new RoomUploadStatus
                    {
                        RoomId = room.Id,
                        Status = "Add Availability Message Error",
                        Message = exception.Message
                    });
                    _logger.LogError(exception, "Error occurred while trying to adding availability messages");                    
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
                _roomUploadStatusRepository.SetStatus(new RoomUploadStatus
                {
                    RoomId = newRoom.Id,
                    Status = "Add Availability Message Error",
                    Message = exception.Message
                });
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
