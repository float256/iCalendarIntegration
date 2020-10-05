using System;

namespace CalendarIntegrationCore.Services.DataSaving
{
    public class RoomAvailabilityInitializationHandlerException: Exception
    {
        public RoomAvailabilityInitializationHandlerException(): base() { }
        public RoomAvailabilityInitializationHandlerException(string message) : base(message) { }
        public RoomAvailabilityInitializationHandlerException(string message, Exception inner) : base(message, inner) { }
    }
}