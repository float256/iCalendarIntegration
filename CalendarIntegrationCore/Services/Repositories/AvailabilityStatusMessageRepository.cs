using System.Collections.Generic;
using System.Linq;
using CalendarIntegrationCore.Models;
using Microsoft.EntityFrameworkCore;

namespace CalendarIntegrationCore.Services.Repositories
{
    public class AvailabilityStatusMessageRepository: IAvailabilityStatusMessageRepository
    {
        private readonly ApplicationContext _context;

        public int Count => _context.AvailabilityStatusMessageSet.Count();

        public AvailabilityStatusMessageRepository(ApplicationContext context)
        {
            _context = context;
        }
        
        public void Add(AvailabilityStatusMessage availabilityStatusMessage)
        {
            _context.AvailabilityStatusMessageSet.Add(availabilityStatusMessage);
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            AvailabilityStatusMessage availabilityStatusMessage = new AvailabilityStatusMessage { Id = id };
            Delete(availabilityStatusMessage);
        }

        public void Delete(AvailabilityStatusMessage availabilityStatusMessage)
        {
            _context.AvailabilityStatusMessageSet.Remove(availabilityStatusMessage);
            _context.SaveChanges();
        }

        public void DeleteMultiple(List<AvailabilityStatusMessage> dateChangeStatuses)
        {
            _context.AvailabilityStatusMessageSet.RemoveRange(dateChangeStatuses);
            _context.SaveChanges();  
        }

        public AvailabilityStatusMessage Get(int id)
        {
            return _context.AvailabilityStatusMessageSet.SingleOrDefault(x => x.Id == id);
        }

        public List<AvailabilityStatusMessage> GetByRoom(int roomId)
        {
            return _context.AvailabilityStatusMessageSet.Where(dateChangeStatus => dateChangeStatus.RoomId == roomId).ToList();
        }

        public List<AvailabilityStatusMessage> GetAll()
        {
            return _context.AvailabilityStatusMessageSet.ToList();
        }

        public List<AvailabilityStatusMessage> GetTop(int numberOfValues)
        {
            return _context.AvailabilityStatusMessageSet.OrderBy(elem => elem.Id)
                .Take(numberOfValues).ToList();
        }
        
        public AvailabilityStatusMessage GetFirst()
        {
            return _context.AvailabilityStatusMessageSet.First();
        }
        
        public AvailabilityStatusMessage GetLast()
        {
            return _context.AvailabilityStatusMessageSet.Last();
        }
    }
}