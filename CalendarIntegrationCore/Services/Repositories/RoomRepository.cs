using CalendarIntegrationCore.Models;
using CalendarIntegrationCore.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;

namespace CalendarIntegrationCore.Services.Repositories
{
    public class RoomRepository: IRoomRepository
    {
        private ApplicationContext _context;

        public RoomRepository(ApplicationContext context)
        {
            _context = context;
        }

        public Room Get(int id)
        {
            return _context.RoomSet.Where(x => x.Id == id).SingleOrDefault();
        }

        public List<Room> GetByHotelId(int hotelId)
        {
            return _context.RoomSet.Where(room => room.HotelId == hotelId).ToList();
        }

        public List<Room> GetAll()
        {
            return _context.RoomSet.ToList();
        }

        public void Add(Room room)
        {
            _context.RoomSet.Add(room);
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            Room room = new Room { Id = id };
            _context.RoomSet.Remove(room);
            _context.SaveChanges();
        }

        public void Update(Room room)
        {
            _context.RoomSet.Update(room);
            _context.SaveChanges();
        }
    }
}
