using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CalendarIntegrationCore.Models;
using CalendarIntegrationCore.Services;
using CalendarIntegrationCore.Services.Repositories;
using CalendarIntegrationWeb.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CalendarIntegrationWeb.Controllers
{
    [Route("api/Hotel")]
    [ApiController]
    public class HotelController : ControllerBase
    {
        private IHotelRepository _hotelRepository;

        public HotelController(IHotelRepository hotelRepository)
        {
            _hotelRepository = hotelRepository;
        }

        [HttpGet("{id:int}")]
        public ActionResult<Hotel> Get(int id)
        {
            Hotel hotel = _hotelRepository.Get(id);
            if (hotel != default)
            {
                return Ok(new HotelDto
                {
                    Id = hotel.Id,
                    HotelCode = hotel.HotelCode,
                    Login = hotel.Login,
                    Name = hotel.Name,
                    Password = hotel.Password
                });
            }
            else
            {
                return NotFound();
            }
        }

        [HttpGet("GetAll")]
        public ActionResult<List<HotelDto>> GetAll()
        {
            return _hotelRepository.GetAll().Select(
                elem => new HotelDto
                {
                    Id = elem.Id,
                    HotelCode = elem.HotelCode,
                    Login = elem.Login,
                    Name = elem.Name,
                    Password = elem.Password
                }).ToList();
        }

        [HttpPost("Add")]
        public ActionResult<Hotel> Add(HotelDto hotelDto)
        {
            Hotel hotel = new Hotel
            {
                Id = hotelDto.Id,
                HotelCode = hotelDto.HotelCode,
                Login = hotelDto.Login,
                Name = hotelDto.Name,
                Password = hotelDto.Password
            };
            _hotelRepository.Add(hotel);
            return Ok(hotel);
        }

        [HttpPost("Update")]
        public void Update(HotelDto hotelDto)
        {
            _hotelRepository.Update(new Hotel
            {
                Id = hotelDto.Id,
                HotelCode = hotelDto.HotelCode,
                Login = hotelDto.Login,
                Name = hotelDto.Name,
                Password = hotelDto.Password
            });
        }

        [HttpPost("Delete")]
        public void Delete([FromBody] int id)
        {
            _hotelRepository.Delete(id);
        }
    }
}
