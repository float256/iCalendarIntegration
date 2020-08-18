using CalendarIntegrationCore.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace CalendarIntegrationCore.Services
{
    public class ApplicationContext: DbContext
    {
        public virtual DbSet<Hotel> HotelSet { get; set; }
        public virtual DbSet<Room> RoomSet { get; set; }
        public DbSet<BookingInfo> BookingInfoSet { get; set; }

        public ApplicationContext(DbContextOptions<ApplicationContext> options): base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder builder) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Hotel>(entity => 
            {
                entity.ToTable("hotel");
            });

            builder.Entity<Room>(entity =>
            {
                entity.ToTable("room");
            });

            builder.Entity<BookingInfo>(entity =>
            {
                entity.ToTable("booking_info");
            });
        }
    }
}
