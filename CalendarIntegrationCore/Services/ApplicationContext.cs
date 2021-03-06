﻿using CalendarIntegrationCore.Models;
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
        public virtual DbSet<BookingInfo> BookingInfoSet { get; set; }
        public virtual DbSet<RoomUploadStatus> RoomUploadStatusSet { get; set; }
        public DbSet<AvailabilityStatusMessage> AvailabilityStatusMessageSet { get; set; }

        public ApplicationContext(DbContextOptions<ApplicationContext> options): base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder builder) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Hotel>().ToTable("hotel");
            builder.Entity<Room>().ToTable("room");
            builder.Entity<BookingInfo>().ToTable("booking_info");
            builder.Entity<AvailabilityStatusMessage>().ToTable("availability_status_message");
            builder.Entity<RoomUploadStatus>().ToTable("room_upload_status");
            builder.Entity<AvailabilityStatusMessage>().Property(c => c.State).HasConversion(
                entity => (int) entity,
                entity => (BookingLimitType) entity);
        }
    }
}