using CalendarIntegrationCore.Models;

namespace CalendarIntegrationCore.Services.DataDownloading
{
    public interface IAvailabilityInfoSaver
    {
        void SaveChanges(BookingInfoChanges changes);
        void AddAvailabilityMessagesForRoomInQueue(Room room, bool isFillGaps = false);
    }
}
