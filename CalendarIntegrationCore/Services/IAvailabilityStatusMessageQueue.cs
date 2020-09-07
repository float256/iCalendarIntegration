using System.Collections.Generic;
using CalendarIntegrationCore.Models;

namespace CalendarIntegrationCore.Services
{
    public interface IAvailabilityStatusMessageQueue
    {
        int Count { get; }
        void Enqueue(AvailabilityStatusMessage availabilityStatusMessage);
        void EnqueueMultiple(List<AvailabilityStatusMessage> availabilityStatusMessages);
        AvailabilityStatusMessage Peek();
        AvailabilityStatusMessage Dequeue();
        List<AvailabilityStatusMessage> DequeueMultiple(int numberOfElements);
    }
}