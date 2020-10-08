using System;
using System.Collections.Generic;
using CalendarIntegrationCore.Models;
using CalendarIntegrationCore.Services.DataProcessing;
using Xunit;

namespace CalendarIntegrationCore.Tests.Services.DataProcessing
{
    public class BookingInfoDataProcessorTests
    {
        [Fact]
        public void BookingInfoDataProcessor_GetChanges_CalendarWithPartOfPreviousAndNewDates_CorrectBookingInfoChanges()
        {
            // Arrange
            int roomId = 1;
            BookingInfoChanges expected = new BookingInfoChanges
            {
                AddedBookingInfo = new List<BookingInfo>
                {
                    new BookingInfo
                    {
                        RoomId = roomId,
                        StartBooking = new DateTime(1953, 2, 12),
                        EndBooking = new DateTime(1953, 6, 3),
                    },
                    new BookingInfo
                    {
                        RoomId = roomId,
                        StartBooking = new DateTime(1984, 12, 30),
                        EndBooking = new DateTime(1984, 12, 31),
                    }
                },
                RemovedBookingInfo = new List<BookingInfo>
                {
                    new BookingInfo
                    {
                        Id = 1,
                        RoomId = roomId,
                        StartBooking = new DateTime(2020, 3, 14),
                        EndBooking = new DateTime(2020, 3, 17),
                    },
                    new BookingInfo
                    {
                        Id = 3,
                        RoomId = roomId,
                        StartBooking = new DateTime(2021, 5, 14),
                        EndBooking = new DateTime(2021, 5, 17),
                    }
                }
            };
            List<BookingInfo> initialAvailabilityInfo = new List<BookingInfo>
            {
                new BookingInfo
                {
                    Id = 1,
                    RoomId = roomId,
                    StartBooking = new DateTime(2020, 3, 14),
                    EndBooking = new DateTime(2020, 3, 17),
                },
                new BookingInfo
                {
                    Id = 2,
                    RoomId = roomId,
                    StartBooking = new DateTime(2019, 4, 16),
                    EndBooking = new DateTime(2019, 4, 21),
                },
                new BookingInfo
                {
                    Id = 3,
                    RoomId = roomId,
                    StartBooking = new DateTime(2021, 5, 14),
                    EndBooking = new DateTime(2021, 5, 17),
                },
            };
            List<BookingInfo> newAvailabilityInfo = new List<BookingInfo>
            {
                new BookingInfo
                {
                    RoomId = roomId,
                    StartBooking = new DateTime(2019, 4, 16),
                    EndBooking = new DateTime(2019, 4, 21),
                },
                new BookingInfo
                {
                    RoomId = roomId,
                    StartBooking = new DateTime(1984, 12, 30),
                    EndBooking = new DateTime(1984, 12, 31),
                },
                new BookingInfo
                {
                    RoomId = roomId,
                    StartBooking = new DateTime(1953, 2, 12),
                    EndBooking = new DateTime(1953, 6, 3),
                }
            };

            // Act
            BookingInfoDataProcessor dataProcessor = new BookingInfoDataProcessor();
            BookingInfoChanges actual = dataProcessor.GetChanges(newAvailabilityInfo, initialAvailabilityInfo);

            // Assert
            Assert.Equal(expected.AddedBookingInfo.Count, actual.AddedBookingInfo.Count);
            Assert.Equal(expected.RemovedBookingInfo.Count, actual.RemovedBookingInfo.Count);
            for (int i = 0; i < expected.AddedBookingInfo.Count; i++)
            {
                var currExpectedBookingInfo = expected.AddedBookingInfo[i];
                var currActualBookingInfo = actual.AddedBookingInfo[i];
                Assert.Equal(currExpectedBookingInfo.StartBooking, currActualBookingInfo.StartBooking);
                Assert.Equal(currExpectedBookingInfo.EndBooking, currActualBookingInfo.EndBooking);
                Assert.Equal(currExpectedBookingInfo.RoomId, currActualBookingInfo.RoomId);
            }
            for (int i = 0; i < expected.RemovedBookingInfo.Count; i++)
            {
                var currExpectedBookingInfo = expected.RemovedBookingInfo[i];
                var currActualBookingInfo = actual.RemovedBookingInfo[i];
                Assert.Equal(currExpectedBookingInfo.Id, currActualBookingInfo.Id);
                Assert.Equal(currExpectedBookingInfo.StartBooking, currActualBookingInfo.StartBooking);
                Assert.Equal(currExpectedBookingInfo.EndBooking, currActualBookingInfo.EndBooking);
                Assert.Equal(currExpectedBookingInfo.RoomId, currActualBookingInfo.RoomId);
            }
        }

        [Fact]
        public void BookingInfoDataProcessor_GetChanges_CalendarWithoutDatesAndWithoutPreviousDates_EmptyBookingInfoChangesObject()
        {
            // Arrange
            List<BookingInfo> newAvailabilityInfo = new List<BookingInfo> { };
            List<BookingInfo> initialAvailabilityInfo = new List<BookingInfo>();
            BookingInfoChanges expected = new BookingInfoChanges();
            
            // Act
            BookingInfoDataProcessor infoSender = new BookingInfoDataProcessor();
            BookingInfoChanges actual = infoSender.GetChanges(newAvailabilityInfo, initialAvailabilityInfo);

            // Assert
            Assert.Equal(expected.AddedBookingInfo.Count, actual.AddedBookingInfo.Count);
            Assert.Equal(expected.RemovedBookingInfo.Count, actual.RemovedBookingInfo.Count);
        }
    }
}
