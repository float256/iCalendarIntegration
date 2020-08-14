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

        /// <summary>
        /// Метод возвращает строку из базы данных, используя id строки для поиска
        /// </summary>
        /// <param name="id">Id строки в базе данных</param>
        /// <returns>Объект типа BookingInfo, содержащий значения строки с указанным Id</returns>
        public BookingInfo Get(int id)
        {
            return _context.BookingInfoSet.Where(x => x.Id == id).SingleOrDefault();
        }

        /// <summary>
        /// Метод возвращает всю информацию о брони комнаты, используя Id этой комнаты.
        /// Также, все значения сортируются по дню начала бронирования (StartBooking)
        /// </summary>
        /// <param name="roomId">Id комнаты</param>
        /// <returns>Список, содержащий всю информацию о брони комнаты</returns>
        public List<BookingInfo> GetByRoomId(int roomId)
        {
            List<BookingInfo> bookingInfoForRoom = _context.BookingInfoSet.Where(bookingInfo => bookingInfo.RoomId == roomId).ToList();
            bookingInfoForRoom.Sort((firstValue, secondValue) => firstValue.StartBooking.CompareTo(secondValue.StartBooking));
            return bookingInfoForRoom;
        }

        /// <summary>
        /// Метод возвращает все строки из таблицы booking_info. 
        /// Также, все значения сортируются по дню начала бронирования (StartBooking)
        /// </summary>
        /// <returns>Список, содержащий все строки</returns>
        public List<BookingInfo> GetAll()
        {
            List<BookingInfo> allBookingInfo = _context.BookingInfoSet.ToList();
            allBookingInfo.Sort((firstValue, secondValue) => firstValue.StartBooking.CompareTo(secondValue.StartBooking));
            return allBookingInfo;
        }

        /// <summary>
        /// Данный метод добавляет в базу данных строку, используя значение из передаваемого объекта.
        /// Также, в поле Id объекта будет добавлено значение Id созданной строки
        /// </summary>
        /// <param name="room">Объект типа BookingInfo, который будет использоваться для добавления строки в базу данных</param>
        public void Add(BookingInfo bookingInfo)
        {
            _context.BookingInfoSet.Add(bookingInfo);
            _context.SaveChanges();
        }

        /// <summary>
        /// Данный метод обновляет строку в базе данных с id, указанном в передаваемом объекте. 
        /// Обновлены будут только поля, значения которых в передаваемом объекте не равны значениям
        /// по умолчанию для типов данных объектов
        /// </summary>
        /// <param name="bookingInfo">Объект типа BookingInfo, который будет использоваться для обновления</param>
        public void Update(BookingInfo bookingInfo)
        {
            BookingInfo entity = _context.BookingInfoSet.First(item => item.Id == bookingInfo.Id);
            if (bookingInfo.RoomId != default)
            {
                entity.RoomId = bookingInfo.RoomId;
            }
            if (bookingInfo.StartBooking != default)
            {
                entity.StartBooking = bookingInfo.StartBooking;
            }
            if (bookingInfo.EndBooking != default)
            {
                entity.EndBooking = bookingInfo.EndBooking;
            }
            _context.SaveChanges();
        }

        /// <summary>
        /// Метод удаляет информацию о бронировании комнаты из базы данных
        /// </summary>
        /// <param name="id">Id записи о бронировании в базе данных</param>
        public void Delete(int id)
        {
            BookingInfo bookingInfo = new BookingInfo { Id = id };
            _context.BookingInfoSet.Remove(bookingInfo);
            _context.SaveChanges();
        }
    }
}
