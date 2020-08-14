using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CalendarIntegrationCore.Models;
using CalendarIntegrationCore.Services.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CalendarIntegrationWeb.Controllers
{
    [Route("api/BookingInfo")]
    [ApiController]
    public class BookingInfoController : ControllerBase
    {
        private IBookingInfoRepository _bookingInfoRepository;
        private IRoomRepository _roomRepository;

        public BookingInfoController(IBookingInfoRepository bookingInfoRepository, IRoomRepository roomRepository)
        {
            _bookingInfoRepository = bookingInfoRepository;
            _roomRepository = roomRepository;
        }

        [HttpGet("{id:int}")]
        public ActionResult<BookingInfo> Get(int id)
        {
            BookingInfo result = _bookingInfoRepository.Get(id);
            if (result != default)
            {
                return Ok(result);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpGet("GetAll")]
        public ActionResult<BookingInfo> GetAll()
        {
            return Ok(_bookingInfoRepository.GetAll());
        }

        [HttpGet("GetByRoomId/{roomId:int}")]
        public ActionResult<List<BookingInfo>> GetAllForRoom(int roomId)
        {
            List<BookingInfo> result = _bookingInfoRepository.GetByRoomId(roomId);
            if (result.Count > 0)
            {
                return Ok(result);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpPost("Add")]
        public ActionResult<BookingInfo> Add(BookingInfo bookingInfo)
        {
            if (_roomRepository.Get(bookingInfo.RoomId) != default)
            {
                _bookingInfoRepository.Add(bookingInfo);
                return Ok(bookingInfo);
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpPost("Update")]
        public void Update(BookingInfo bookingInfo)
        {
            _bookingInfoRepository.Update(bookingInfo);
        }

        [HttpPost("Delete")]
        public void Delete([FromBody] int id)
        {
            _bookingInfoRepository.Delete(id);
        }
    }
}
