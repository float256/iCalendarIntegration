using CalendarIntegrationCore.Models;
using CalendarIntegrationCore.Services.Repositories;
using Microsoft.AspNetCore.Mvc;


namespace CalendarIntegrationWeb.Controllers
{
    [Route("api/RoomUploadStatus")]
    [ApiController]
    public class RoomUploadStatusController : ControllerBase
    {
        private readonly IRoomUploadStatusRepository _roomUploadStatusRepository;

        public RoomUploadStatusController(IRoomUploadStatusRepository roomUploadStatusRepository)
        {
            _roomUploadStatusRepository = roomUploadStatusRepository;
        }
        
        [HttpGet("{id:int}")]
        public ActionResult<RoomUploadStatus> Get(int id)
        {
            return Ok(_roomUploadStatusRepository.Get(id));
        }

        [HttpGet("GetByRoomId/{roomId:int}")]
        public ActionResult<RoomUploadStatus> GetByRoomId(int roomId)
        {
            return Ok(_roomUploadStatusRepository.GetByRoomId(roomId));
        }
    }
}