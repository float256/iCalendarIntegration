using System.Collections.Generic;
using System.Threading.Tasks;
using CalendarIntegrationCore.Models;

namespace CalendarIntegrationCore.Services
{
    public interface IAvailabilityInfoSender
    {
        void SaveAndSendAllInfo();
        string GetCalendarByUrl(string url);
        void SaveChanges(BookingInfoChanges changes);
        BookingInfoChanges GetChanges(string calendar, int roomId);
    }
}