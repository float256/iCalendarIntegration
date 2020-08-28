using CalendarIntegrationCore.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace CalendarIntegrationCore.Services
{
    public interface IAvailabilityInfoSaver
    {
        void SaveChanges(BookingInfoChanges changes);
    }
}
