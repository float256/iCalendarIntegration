using System.Collections.Generic;
using CalendarIntegrationCore.Models;

namespace CalendarIntegrationCore.Services.DataSaving
{
    public interface IAvailabilityStatusMessageQueue
    {
        void Enqueue(AvailabilityStatusMessage availabilityStatusMessage);
        void EnqueueMultiple(List<AvailabilityStatusMessage> availabilityStatusMessages);
        AvailabilityStatusMessage Peek();
        List<AvailabilityStatusMessage> PeekMultiple(int numberOfElements);
        AvailabilityStatusMessage Dequeue();
        void DequeueMultiple(int numberOfElements);
    }
}