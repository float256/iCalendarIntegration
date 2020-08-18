using CalendarIntegrationCore.Models;
using CalendarIntegrationCore.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CalendarIntegrationCore.Services.Repositories
{
    public class BookingInfoRepository: IBookingInfoRepository
    {
        private ApplicationContext _context;

        public BookingInfoRepository(ApplicationContext context)
        {
            _context = context;
        }

        public BookingInfo Get(int id)
        {
            return _context.BookingInfoSet.Where(x => x.Id == id).SingleOrDefault();
        }

        public List<BookingInfo> GetByRoomId(int roomId)
        {
            List<BookingInfo> bookingInfoForRoom = _context.BookingInfoSet.Where(bookingInfo => bookingInfo.RoomId == roomId).ToList();
            bookingInfoForRoom.OrderBy(elem => elem.StartBooking);
            return bookingInfoForRoom;
        }

        public List<BookingInfo> GetAll()
        {
            List<BookingInfo> allBookingInfo = _context.BookingInfoSet.ToList();
            allBookingInfo.OrderBy(elem => elem.StartBooking);
            return allBookingInfo;
        }

        public void Add(BookingInfo bookingInfo)
        {
            _context.BookingInfoSet.Add(bookingInfo);
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            BookingInfo bookingInfo = new BookingInfo { Id = id };
            _context.BookingInfoSet.Remove(bookingInfo);
            _context.SaveChanges();
        }

        public void Update(BookingInfo bookingInfo)
        {
            _context.BookingInfoSet.Update(bookingInfo);
            _context.SaveChanges();
        }
    }
}
