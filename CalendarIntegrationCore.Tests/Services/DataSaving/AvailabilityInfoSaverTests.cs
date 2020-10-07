using System;
using System.Collections.Generic;
using CalendarIntegrationCore.Models;
using CalendarIntegrationCore.Services.DataSaving;
using CalendarIntegrationCore.Services.Repositories;
using Moq;
using Xunit;

namespace CalendarIntegrationCore.Tests.Services.DataSaving
{
    public class AvailabilityInfoSaverTests
    {
        [Fact]
        public void AvailabilityInfoSaver_SaveChanges_NotEmptyBookingInfoChangesObject_ChangesHaveBeenWrittenToDatabase()
        {
            // Arrange
            int roomId = 1;
            List<BookingInfo> expectedBookingInfoRepositoryData = new List<BookingInfo>
            {
                new BookingInfo
                {
                    Id = 1,
                    RoomId = roomId,
                    StartBooking = new DateTime(2012, 3, 24),
                    EndBooking = new DateTime(2012, 3, 27),
                },
                new BookingInfo
                {
                    Id = 4,
                    RoomId = roomId,
                    StartBooking = new DateTime(1953, 2, 12),
                    EndBooking = new DateTime(1953, 6, 3),
                },
                new BookingInfo
                {
                    Id = 5,
                    RoomId = roomId,
                    StartBooking = new DateTime(1984, 12, 30),
                    EndBooking = new DateTime(1984, 12, 31),
                }
            };
            List<BookingInfo> actualBookingInfoRepositoryData = new List<BookingInfo>
            {
                new BookingInfo
                {
                    Id = 1,
                    RoomId = roomId,
                    StartBooking = new DateTime(2012, 3, 24),
                    EndBooking = new DateTime(2012, 3, 27),
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
            int maxBookingInfoId = 3;
            BookingInfoChanges changes = new BookingInfoChanges
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
                    }
                }
            };

            Mock<IBookingInfoRepository> mockBookingInfoRepository = new Mock<IBookingInfoRepository>(MockBehavior.Strict);
            
            mockBookingInfoRepository.Setup(repository => repository.Add(It.IsAny<BookingInfo>()))
                .Callback((BookingInfo bookingInfo) =>
                {
                    maxBookingInfoId++;
                    bookingInfo.Id = maxBookingInfoId;
                    actualBookingInfoRepositoryData.Add(bookingInfo);
                });
            mockBookingInfoRepository.Setup(repository => repository.Delete(It.IsAny<BookingInfo>()))
                .Callback((BookingInfo bookingInfo) =>
                {
                    actualBookingInfoRepositoryData.RemoveAll(elem => elem.Id == bookingInfo.Id);
                });

            // Act
            BookingInfoSaver infoSaver = new BookingInfoSaver(mockBookingInfoRepository.Object);
            infoSaver.SaveChanges(changes);

            // Assert
            Assert.Equal(expectedBookingInfoRepositoryData.Count, actualBookingInfoRepositoryData.Count);
            for (int i = 0; i < expectedBookingInfoRepositoryData.Count; i++)
            {
                BookingInfo currExpectedBookingInfo = expectedBookingInfoRepositoryData[i];
                BookingInfo currActualBookingInfo = actualBookingInfoRepositoryData[i];

                Assert.Equal(currExpectedBookingInfo.Id, currActualBookingInfo.Id);
                Assert.Equal(currExpectedBookingInfo.StartBooking, currActualBookingInfo.StartBooking);
                Assert.Equal(currExpectedBookingInfo.EndBooking, currActualBookingInfo.EndBooking);
                Assert.Equal(currExpectedBookingInfo.RoomId, currActualBookingInfo.RoomId);
            }
        }

        [Fact]
        public void AvailabilityInfoSaver_SaveChanges_EmptyBookingInfoChangesObject_NoChanges()
        {
            // Arrange
            int roomId = 1;
            List<BookingInfo> expectedBookingInfoRepositoryData = new List<BookingInfo>
            {
                new BookingInfo
                {
                    Id = 1,
                    RoomId = roomId,
                    StartBooking = new DateTime(2012, 3, 24),
                    EndBooking = new DateTime(2012, 3, 27),
                },
                new BookingInfo
                {
                    Id = 2,
                    RoomId = roomId,
                    StartBooking = new DateTime(2019, 4, 16),
                    EndBooking = new DateTime(2019, 4, 21),
                },
            };
            List<BookingInfo> actualBookingInfoRepositoryData = new List<BookingInfo>
            {
                new BookingInfo
                {
                    Id = 1,
                    RoomId = roomId,
                    StartBooking = new DateTime(2012, 3, 24),
                    EndBooking = new DateTime(2012, 3, 27),
                },
                new BookingInfo
                {
                    Id = 2,
                    RoomId = roomId,
                    StartBooking = new DateTime(2019, 4, 16),
                    EndBooking = new DateTime(2019, 4, 21),
                },
            };
            int maxBookingInfoId = 2;
            BookingInfoChanges changes = new BookingInfoChanges();
            
            Mock<IBookingInfoRepository> mockBookingInfoRepository = new Mock<IBookingInfoRepository>(MockBehavior.Strict);
            
            mockBookingInfoRepository.Setup(repository => repository.Add(It.IsAny<BookingInfo>()))
                .Callback((BookingInfo bookingInfo) =>
                {
                    maxBookingInfoId++;
                    bookingInfo.Id = maxBookingInfoId;
                    actualBookingInfoRepositoryData.Add(bookingInfo);
                });
            mockBookingInfoRepository.Setup(repository => repository.Delete(It.IsAny<int>()))
                .Callback((int id) =>
                {
                    actualBookingInfoRepositoryData.RemoveAll(elem => elem.Id == id);
                });
    
            // Act
            BookingInfoSaver infoSaver = new BookingInfoSaver(mockBookingInfoRepository.Object);
            infoSaver.SaveChanges(changes);

            // Assert
            Assert.Equal(expectedBookingInfoRepositoryData.Count, actualBookingInfoRepositoryData.Count);
            for (int i = 0; i < expectedBookingInfoRepositoryData.Count; i++)
            {
                BookingInfo currExpectedBookingInfo = expectedBookingInfoRepositoryData[i];
                BookingInfo currActualBookingInfo = actualBookingInfoRepositoryData[i];

                Assert.Equal(currExpectedBookingInfo.Id, currActualBookingInfo.Id);
                Assert.Equal(currExpectedBookingInfo.StartBooking, currActualBookingInfo.StartBooking);
                Assert.Equal(currExpectedBookingInfo.EndBooking, currActualBookingInfo.EndBooking);
                Assert.Equal(currExpectedBookingInfo.RoomId, currActualBookingInfo.RoomId);
            }
        }
    }
}
