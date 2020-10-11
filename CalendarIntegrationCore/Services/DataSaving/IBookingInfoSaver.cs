using CalendarIntegrationCore.Models;

namespace CalendarIntegrationCore.Services.DataSaving
{
    public interface IBookingInfoSaver
    {
        void SaveChanges(BookingInfoChanges changes);
    }
}
