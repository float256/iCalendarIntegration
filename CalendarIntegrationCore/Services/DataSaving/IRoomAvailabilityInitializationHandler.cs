using CalendarIntegrationCore.Models;

namespace CalendarIntegrationCore.Services.DataSaving
{
    public interface IRoomAvailabilityInitializationHandler
    {
        void AddAvailabilityMessagesForRoomInQueue(Room room);
    }
}