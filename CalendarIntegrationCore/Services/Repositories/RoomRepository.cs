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

        /// <summary>
        /// Метод возвращает строку из базы данных, используя id строки для поиска
        /// </summary>
        /// <param name="id">Id строки в базе данных</param>
        /// <returns>Объект типа Room, содержащий значения строки с указанным Id</returns>
        public Room Get(int id)
        {
            return _context.RoomSet.Where(x => x.Id == id).SingleOrDefault();
        }

        /// <summary>
        /// Метод возвращает информацию о всех комнатах, находящихся в отеле с заданным Id
        /// </summary>
        /// <param name="hotelId">Id отеля</param>
        /// <returns>Список, содержащий информацию обо всех комнатах отеля</returns>
        public List<Room> GetByHotelId(int hotelId)
        {
            return _context.RoomSet.Where(room => room.HotelId == hotelId).ToList();
        }

        /// <summary>
        /// Метод возвращает все строки из таблицы room
        /// </summary>
        /// <returns>Список, содержащий все строки</returns>
        public List<Room> GetAll()
        {
            return _context.RoomSet.ToList();
        }

        /// <summary>
        /// Данный метод добавляет в базу данных строку, используя значение из передаваемого объекта.
        /// Также, в поле Id объекта будет добавлено значение Id созданной строки
        /// </summary>
        /// <param name="room">Объект типа Room, который будет использоваться для добавления строки в базу данных</param>
        public void Add(Room room)
        {
            _context.RoomSet.Add(room);
            _context.SaveChanges();
        }

        /// <summary>
        /// Данный метод обновляет строку в базе данных с id, указанном в передаваемом объекте. 
        /// Обновлены будут только поля, значения которых в передаваемом объекте не равны значениям
        /// по умолчанию для типов данных объектов
        /// </summary>
        /// <param name="room">Объект типа Room, который будет использоваться для обновления</param>
        public void Update(Room room)
        {
            Room entity = _context.RoomSet.First(item => item.Id == room.Id);
            _context.RoomSet.Attach(entity);
            if (room.HotelId != default)
            {
                entity.HotelId = room.HotelId;
            }
            if (room.TLApiCode != default)
            {
                entity.TLApiCode = room.TLApiCode;
            }
            if (room.Url != default)
            {
                entity.Url = room.Url;
            }
            if (room.Name != default)
            {
                entity.Name = room.Name;
            }
            _context.SaveChanges();
        }

        /// <summary>
        /// Метод удаляет из базы данных информацию о комнате и всю информацию о бронировании комнаты
        /// </summary>
        /// <param name="id">Id комнаты в базе данных</param>
        public void Delete(int id)
        {
            Room room = new Room { Id = id };
            IQueryable allBookingInfo = _context.BookingInfoSet.Where(x => x.RoomId == id);
            _context.RemoveRange(allBookingInfo);
            _context.RoomSet.Remove(room);
            _context.SaveChanges();
        }
    }
}
