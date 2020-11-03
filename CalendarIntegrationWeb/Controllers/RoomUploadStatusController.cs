using CalendarIntegrationCore.Models;
using CalendarIntegrationCore.Services.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace CalendarIntegrationWeb.Controllers
{
    [Route("api/RoomUploadStatus")]
    [ApiController]
    public class RoomUploadStatusController
    {
        private readonly IRoomUploadStatusRepository _roomUploadStatusRepository;

        public RoomUploadStatusController(IRoomUploadStatusRepository roomUploadStatusRepository)
        {
            _roomUploadStatusRepository = roomUploadStatusRepository;
        }

        [HttpGet("{id:int}")]
        public ActionResult<RoomUploadStatus> Get(int id)
        {
            return _roomUploadStatusRepository.Get(id);
        }

        [HttpGet("GetByRoomId/{id:int}")]
        public ActionResult<RoomUploadStatus> GetByRoomId(int id)
        {
            return _roomUploadStatusRepository.GetByRoomId(id);
        }
    }
}
