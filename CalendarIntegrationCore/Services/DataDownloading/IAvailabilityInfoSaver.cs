using CalendarIntegrationCore.Models;

namespace CalendarIntegrationCore.Services.DataDownloading
{
    public interface IAvailabilityInfoSaver
    {
        void SaveChanges(BookingInfoChanges changes);
        void AddAllBookingInfoForRoomInQueue(Room room, bool isFillGaps = false);
    }
}
