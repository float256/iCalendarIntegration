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
        
        /// <summary>
        /// Метод возвращает строку из базы данных, используя id строки для поиска
        /// </summary>
        /// <param name="id">Id строки в базе данных</param>
        /// <returns>Объект типа Hotel, содержащий значения строки с указанным Id</returns>
        public Hotel Get(int id)
        {
            return _context.HotelSet.Where(x => x.Id == id).SingleOrDefault();
        }

        /// <summary>
        /// Метод возвращает все строки из таблицы hotel
        /// </summary>
        /// <returns>Список, содержащий все строки</returns>
        public List<Hotel> GetAll()
        {
            return _context.HotelSet.ToList();
        }

        /// <summary>
        /// Данный метод добавляет в базу данных строку, используя значение из передаваемого объекта.
        /// Также, в поле Id объекта будет добавлено значение Id созданной строки
        /// </summary>
        /// <param name="hotel">Объект типа Hotel, который будет использоваться для добавления строки в базу данных</param>
        public void Add(Hotel hotel)
        {
            _context.HotelSet.Add(hotel);
            _context.SaveChanges();
        }

        /// <summary>
        /// Данный метод обновляет строку в базе данных с id, указанном в передаваемом объекте. 
        /// Обновлены будут только поля, значения которых в передаваемом объекте не равны значениям
        /// по умолчанию для типов данных объектов
        /// </summary>
        /// <param name="hotel">Объект типа Hotel, который будет использоваться для обновления</param>
        public void Update(Hotel hotel)
        {
            Hotel entity = _context.HotelSet.First(item => item.Id == hotel.Id);
            _context.HotelSet.Attach(entity);
            if (hotel.Login != default)
            {
                entity.Login = hotel.Login;
            }
            if (hotel.Name != default)
            {
                entity.Name = hotel.Name;
            }
            if (hotel.Password != default)
            {
                entity.Password = hotel.Password;
            }
            if (hotel.HotelCode != default)
            {
                entity.HotelCode = hotel.HotelCode;
            }
            _context.SaveChanges();
        }

        /// <summary>
        /// Метод удаляет из базы данных отель и всю информацию связанную с ним (комнаты и информацию о бронировании комнат)
        /// </summary>
        /// <param name="id">Id отеля в базе данных</param>
        public void Delete(int id)
        {
            Hotel hotel = new Hotel() { Id = id };
            List<Room> allRoomsInHotel = _context.RoomSet.Where(x => x.HotelId == id).ToList();
            foreach (var currRoom in allRoomsInHotel)
            {
                IQueryable<BookingInfo> roomBookingInfo = _context.BookingInfoSet.Where(x => x.RoomId == currRoom.Id);
                if (roomBookingInfo.ToList().Count > 0)
                {
                    _context.RemoveRange(roomBookingInfo);
                }
                _context.RoomSet.Remove(currRoom);
            }
            _context.HotelSet.Remove(hotel);
            _context.SaveChanges();
        }
    }
}
