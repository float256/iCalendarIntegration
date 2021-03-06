﻿using CalendarIntegrationCore.Models;
using CalendarIntegrationCore.Services;
using Microsoft.EntityFrameworkCore;
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
        private readonly ApplicationContext _context;

        public HotelRepository(ApplicationContext context)
        {
            _context = context;
        }
        
        public Hotel Get(int id)
        {
            return _context.HotelSet.SingleOrDefault(x => x.Id == id);
        }

        public List<Hotel> GetMultiple(List<int> hotelIndexes)
        {
            return _context.HotelSet.Where(hotel => hotelIndexes.Contains(hotel.Id)).ToList();
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
            _context.Entry(hotel).State = EntityState.Modified;
            _context.SaveChanges();
        }

        public void Detach(Hotel hotel)
        {
            _context.Entry(hotel).State = EntityState.Detached;
        }
    }
}
