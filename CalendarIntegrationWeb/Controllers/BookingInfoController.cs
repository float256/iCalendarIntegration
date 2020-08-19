using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CalendarIntegrationCore.Models;
using CalendarIntegrationCore.Services.Repositories;
using CalendarIntegrationWeb.Dto;
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
            BookingInfo bookingInfo = _bookingInfoRepository.Get(id);
            if (bookingInfo != default)
            {
                return Ok(new BookingInfoDto
                {
                    Id = bookingInfo.Id,
                    StartBooking = bookingInfo.StartBooking,
                    EndBooking = bookingInfo.EndBooking,
                    RoomId = bookingInfo.RoomId
                });
            }
            else
            {
                return NotFound();
            }
        }

        [HttpGet("GetAll")]
        public ActionResult<BookingInfoDto> GetAll()
        {
            List<BookingInfoDto> allBookingInfo = _bookingInfoRepository.GetAll().Select(
                elem => new BookingInfoDto
                {
                    Id = elem.Id,
                    StartBooking = elem.StartBooking,
                    EndBooking = elem.EndBooking,
                    RoomId = elem.RoomId
                }).ToList();
            return Ok(allBookingInfo);
        }

        [HttpGet("GetByRoomId/{roomId:int}")]
        public ActionResult<List<BookingInfoDto>> GetAllForRoom(int roomId)
        {
            List<BookingInfoDto> result = _bookingInfoRepository.GetByRoomId(roomId).Select(
                elem => new BookingInfoDto 
                {
                    Id = elem.Id,
                    StartBooking = elem.StartBooking,
                    EndBooking = elem.EndBooking,
                    RoomId = elem.RoomId
                }).ToList();
            return Ok(result);
        }

        [HttpPost("Add")]
        public ActionResult<BookingInfo> Add(BookingInfoDto bookingInfoDto)
        {
            if (_roomRepository.Get(bookingInfoDto.RoomId) != default)
            {
                BookingInfo bookingInfo = new BookingInfo
                {
                    Id = bookingInfoDto.Id,
                    EndBooking = bookingInfoDto.EndBooking,
                    StartBooking = bookingInfoDto.StartBooking,
                    RoomId = bookingInfoDto.RoomId
                };
                _bookingInfoRepository.Add(bookingInfo);
                return Ok(bookingInfo);
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpPost("Update")]
        public void Update(BookingInfoDto bookingInfoDto)
        {
            _bookingInfoRepository.Update(new BookingInfo
            {
                Id = bookingInfoDto.Id,
                EndBooking = bookingInfoDto.EndBooking,
                StartBooking = bookingInfoDto.StartBooking,
                RoomId = bookingInfoDto.RoomId
            });
        }

        [HttpPost("Delete")]
        public void Delete([FromBody] int id)
        {
            _bookingInfoRepository.Delete(id);
        }
    }
}
