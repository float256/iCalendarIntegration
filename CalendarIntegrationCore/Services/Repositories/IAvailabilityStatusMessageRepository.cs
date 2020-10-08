using System.Collections.Generic;
using CalendarIntegrationCore.Models;

namespace CalendarIntegrationCore.Services.Repositories
{
    public interface IAvailabilityStatusMessageRepository
    {
        int Count { get; }
        
        void Add(AvailabilityStatusMessage availabilityStatusMessage);
        void Delete(int id);
        void Delete(AvailabilityStatusMessage availabilityStatusMessage);
        void DeleteMultiple(List<AvailabilityStatusMessage> dateChangeStatuses);
        AvailabilityStatusMessage Get(int id);
        List<AvailabilityStatusMessage> GetByRoom(int roomId);
        List<AvailabilityStatusMessage> GetAll();
        List<AvailabilityStatusMessage> GetTop(int numberOfValues);
        AvailabilityStatusMessage GetFirst();
        AvailabilityStatusMessage GetLast();
    }
}