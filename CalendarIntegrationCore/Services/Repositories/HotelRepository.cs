using CalendarIntegrationCore.Models;
using CalendarIntegrationCore.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace CalendarIntegrationCore.Services.Repositories
{
    public class HotelRepository: IHotelRepository
    {
        private ApplicationContext _context;

        public HotelRepository(ApplicationContext context)
        {
            _context = context;
        }
        
        public Hotel Get(int id)
        {
            return _context.HotelSet.Where(x => x.Id == id).SingleOrDefault();
        }

        public List<Hotel> GetAll()
        {
            return _context.HotelSet.ToList();
        }

        public void Add(Hotel hotel)
        {
            _context.HotelSet.Add(hotel);
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            Hotel hotel = new Hotel() { Id = id };
            _context.HotelSet.Remove(hotel);
            _context.SaveChanges();
        }

        public void Update(Hotel hotel)
        {
            _context.HotelSet.Update(hotel);
            _context.SaveChanges();
        }
    }
}
