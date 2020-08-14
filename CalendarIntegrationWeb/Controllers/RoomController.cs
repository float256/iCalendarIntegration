using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CalendarIntegrationCore.Models;
using CalendarIntegrationCore.Services;
using CalendarIntegrationCore.Services.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CalendarIntegrationWeb.Controllers
{
    [Route("api/Room")]
    [ApiController]
    public class RoomController : ControllerBase
    {
        private IRoomRepository _roomRepository;
        private IHotelRepository _hotelRepository;

        public RoomController(IRoomRepository roomRepository, IHotelRepository hotelRepository)
        {
            _roomRepository = roomRepository;
            _hotelRepository = hotelRepository;
        }

        [HttpGet("{id:int}")]
        public ActionResult<Room> Get(int id)
        {
            Room result = _roomRepository.Get(id);
            if (result != default)
            {
                return Ok(result);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpGet("GetByHotelId/{hotelId:int}")]
        public ActionResult<List<Room>> GetByHotelId(int hotelId)
        {
            List<Room> result = _roomRepository.GetByHotelId(hotelId);
            if (result.Count > 0)
            {
                return Ok(result);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpGet("GetAll")]
        public ActionResult<List<Room>> GetAll()
        {
            return _roomRepository.GetAll();
        }

        [HttpPost("Add")]
        public ActionResult<Room> Add(Room room)
        {
            if (_hotelRepository.Get(room.HotelId) != default)
            {
                _roomRepository.Add(room);
                return Ok(room);
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpPost("Update")]
        public void Update(Room room)
        {
            _roomRepository.Update(room);
        }

        [HttpPost("Delete")]
        public void Delete([FromBody] int id)
        {
            _roomRepository.Delete(id);
        }
    }
}
