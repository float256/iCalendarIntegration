using System;
using System.Collections.Generic;
using System.Threading;
using CalendarIntegrationCore.Models;
using CalendarIntegrationCore.Services.Repositories;
using CalendarIntegrationWeb.Hubs;
using CalendarIntegrationWeb.Observers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace CalendarIntegrationWeb.Controllers
{
    [Route("api/RoomUploadStatus")]
    [ApiController]
    public class RoomUploadStatusController : ControllerBase
    {
        private readonly IRoomUploadStatusRepository _roomUploadStatusRepository;
        //private IHubContext<RoomUploadStatusHub> _hub;
        private readonly IRoomUploadStatusObserver _roomUploadStatusObserver;

        public RoomUploadStatusController(
            IRoomUploadStatusRepository roomUploadStatusRepository,
            IRoomUploadStatusObserver roomUploadStatusObserver,
            IHubContext<RoomUploadStatusHub> hub )
        {
            _roomUploadStatusRepository = roomUploadStatusRepository;
            //_hub = hub;
            _roomUploadStatusObserver = roomUploadStatusObserver;
        }
        
        //public IActionResult Get()
        //{
        //    //var timerManager = new TimerManager( () => _hub.Clients.All.SendAsync( "transferRoomUploadStatus", new List<int> { 123, 345, 567 } ) );
        //    //return Ok( new { Message = "Request Completed" } );
        //}

        //[HttpGet("GetByRoomId/{roomId:int}")]
        //public ActionResult<RoomUploadStatus> GetByRoomId(int roomId)
        //{
        //    return Ok(_roomUploadStatusRepository.GetByRoomId(roomId));
        //}
    }

    //public class TimerManager
    //{
    //    private Timer _timer;
    //    private AutoResetEvent _autoResetEvent;
    //    private Action _action;
    //    public DateTime TimerStarted { get; }
    //    public TimerManager( Action action )
    //    {
    //        _action = action;
    //        _autoResetEvent = new AutoResetEvent( false );
    //        _timer = new Timer( Execute, _autoResetEvent, 1000, 2000 );
    //        TimerStarted = DateTime.Now;
    //    }
    //    public void Execute( object stateInfo )
    //    {
    //        _action();
    //        if ( ( DateTime.Now - TimerStarted ).Seconds > 60 )
    //        {
    //            _timer.Dispose();
    //        }
    //    }
    //}
}