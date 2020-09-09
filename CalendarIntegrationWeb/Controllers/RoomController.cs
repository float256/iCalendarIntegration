using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CalendarIntegrationCore.Models;
using CalendarIntegrationCore.Services;
using CalendarIntegrationCore.Services.DataDownloading;
using CalendarIntegrationCore.Services.Repositories;
using CalendarIntegrationWeb.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CalendarIntegrationWeb.Controllers
{
    [Route("api/Room")]
    [ApiController]
    public class RoomController : ControllerBase
    {
        private readonly IRoomRepository _roomRepository;
        private readonly IHotelRepository _hotelRepository;
        private readonly IAvailabilityInfoSaver _infoSaver;

        public RoomController(IRoomRepository roomRepository, IHotelRepository hotelRepository, IAvailabilityInfoSaver infoSaver)
        {
            _roomRepository = roomRepository;
            _hotelRepository = hotelRepository;
            _infoSaver = infoSaver;
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
            Room previousRoomValues = _roomRepository.Get(roomDto.Id);
            _roomRepository.Detach(previousRoomValues);
            Room newRoomValues = new Room
            {
                Id = roomDto.Id,
                HotelId = roomDto.HotelId,
                Name = roomDto.Name,
                TLApiCode = roomDto.TLApiCode,
                Url = roomDto.Url
            };
            if (previousRoomValues.TLApiCode != roomDto.TLApiCode)
            {
                _infoSaver.AddAllBookingInfoForRoomInQueue(newRoomValues, isFillGaps: true);
            }
            _roomRepository.Update(newRoomValues);
        }

        [HttpPost("Delete")]
        public void Delete([FromBody] int id)
        {
            _roomRepository.Delete(id);
        }
    }
}
