﻿using CalendarIntegrationCore.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace CalendarIntegrationCore.Services.Repositories
{
    public interface IHotelRepository
    {
        Hotel Get(int id);
        List<Hotel> GetMultiple(List<int> hotelIndexes);
        List<Hotel> GetAll();
        void Add(Hotel hotel);
        void Update(Hotel hotel);
        void Delete(int id);
        void Detach(Hotel hotel);
    }
}
