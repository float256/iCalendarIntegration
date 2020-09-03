using CalendarIntegrationCore.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CalendarIntegrationCore.Services
{
    public interface IAvailabilityInfoSender
    {
        Task SendForOneHotel(BookingInfoChangesForHotel hotelChanges);
    }
}
