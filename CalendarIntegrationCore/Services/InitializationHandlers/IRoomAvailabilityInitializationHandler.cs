using CalendarIntegrationCore.Models;

namespace CalendarIntegrationCore.Services.InitializationHandlers
{
    public interface IRoomAvailabilityInitializationHandler
    {
        void AddAvailabilityMessagesForRoomToQueue(Room room);
    }
}