using CalendarIntegrationCore.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace CalendarIntegrationCore.Services.Repositories
{
    public interface IRoomRepository
    {
        Room Get(int id);
        List<Room> GetMultiple(List<int> roomIndexes);
        List<Room> GetByHotelId(int hotelId);
        List<Room> GetAll();
        void Add(Room room);
        void Update(Room room);
        void Delete(int id);
        void Detach(Room room);
    }
}
