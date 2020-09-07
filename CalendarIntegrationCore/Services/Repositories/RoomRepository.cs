using CalendarIntegrationCore.Models;
using CalendarIntegrationCore.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;

namespace CalendarIntegrationCore.Services.Repositories
{
    public class RoomRepository: IRoomRepository
    {
        private readonly ApplicationContext _context;
        private readonly IAvailabilityInfoSaver _infoSaver;
        
        public RoomRepository(ApplicationContext context, IAvailabilityInfoSaver infoSaver)
        {
            _context = context;
            _infoSaver = infoSaver;
        }

        public Room Get(int id)
        {
            return _context.RoomSet.SingleOrDefault(x => x.Id == id);
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
            Room previousRoomValues = _context.RoomSet.Find(room.Id);
            _context.Entry(previousRoomValues).State = EntityState.Detached;
            if (previousRoomValues.TLApiCode != room.TLApiCode)
            {
                _infoSaver.AddAllBookingInfoForRoomInQueue(room, isFillGaps: true);
            }
            _context.RoomSet.Update(room);
            _context.Entry(room).State = EntityState.Modified;
            _context.SaveChanges();
        }
    }
}