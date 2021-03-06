﻿using CalendarIntegrationCore.Models;
using CalendarIntegrationCore.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CalendarIntegrationCore.Services.Repositories
{
    public class BookingInfoRepository: IBookingInfoRepository
    {
        private readonly ApplicationContext _context;

        public BookingInfoRepository(ApplicationContext context)
        {
            _context = context;
        }

        public BookingInfo Get(int id)
        {
            BookingInfo bookingInfo = _context.BookingInfoSet.SingleOrDefault(x => x.Id == id);
            return bookingInfo;
        }

        public List<BookingInfo> GetByRoomId(int roomId)
        {
            List<BookingInfo> bookingInfoForRoom = _context.BookingInfoSet.Where(bookingInfo => bookingInfo.RoomId == roomId).ToList();
            return bookingInfoForRoom.OrderBy(elem => elem.StartBooking).ToList();
        }

        public List<BookingInfo> GetAll()
        {
            List<BookingInfo> allBookingInfo = _context.BookingInfoSet.ToList();
            return allBookingInfo.OrderBy(elem => elem.StartBooking).ToList();
        }

        public void Add(BookingInfo bookingInfo)
        {
            _context.BookingInfoSet.Add(bookingInfo);
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            BookingInfo bookingInfo = new BookingInfo { Id = id };
            Delete(bookingInfo);
        }

        public void Delete(BookingInfo bookingInfo)
        {
            _context.BookingInfoSet.Remove(bookingInfo);
            _context.SaveChanges();
        }

        public void Update(BookingInfo bookingInfo)
        {
            _context.BookingInfoSet.Update(bookingInfo);
            _context.Entry(bookingInfo).State = EntityState.Modified;
            _context.SaveChanges();
        }
    }
}
