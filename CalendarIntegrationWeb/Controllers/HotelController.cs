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
            Hotel result = _hotelRepository.Get(id);
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
        public ActionResult<List<Hotel>> GetAll()
        {
            return _hotelRepository.GetAll();
        }

        [HttpPost("Add")]
        public ActionResult<Hotel> Add(Hotel hotel)
        {
            _hotelRepository.Add(hotel);
            return Ok(hotel);
        }

        [HttpPost("Update")]
        public void Update(Hotel hotel)
        {
            _hotelRepository.Update(hotel);
        }

        [HttpPost("Delete")]
        public void Delete([FromBody] int id)
        {
            _hotelRepository.Delete(id);
        }
    }
}
