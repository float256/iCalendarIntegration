using System.Linq;
using CalendarIntegrationCore.Models;
using Microsoft.EntityFrameworkCore;

namespace CalendarIntegrationCore.Services.Repositories
{
    public class RoomUploadStatusRepository: IRoomUploadStatusRepository
    {
        
        private readonly ApplicationContext _context;
        
        public RoomUploadStatusRepository(ApplicationContext context)
        {
            _context = context;
        }
        
        public RoomUploadStatus Get(int id)
        {
            return _context.RoomUploadStatusSet.SingleOrDefault(x => x.Id == id);
        }

        public RoomUploadStatus GetByRoomId(int roomId)
        {
            return _context.RoomUploadStatusSet.SingleOrDefault(elem => elem.RoomId == roomId);
        }

        public void SetStatus(RoomUploadStatus newStatus)
        {
            RoomUploadStatus statusFromDb = GetByRoomId(newStatus.RoomId);
            if (statusFromDb != null)
            {
                _context.Entry(statusFromDb).State = EntityState.Detached;
                newStatus.Id = statusFromDb.Id;
                _context.RoomUploadStatusSet.Update(newStatus);
            }
            else
            {
                _context.RoomUploadStatusSet.Add(newStatus);
            }
            _context.SaveChanges();
        }
        
        public void DeleteByRoomId(int roomId)
        {
            _context.RoomUploadStatusSet.RemoveRange(_context.RoomUploadStatusSet.Where(
                elem => elem.RoomId == roomId));
            _context.SaveChanges();
        }
    }
}