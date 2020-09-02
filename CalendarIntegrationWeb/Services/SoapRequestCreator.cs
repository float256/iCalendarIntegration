using CalendarIntegrationCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TLConnect;

namespace CalendarIntegrationWeb.Services
{
    public class SoapRequestCreator: ISoapRequestCreator
    {
        private readonly string _addedBookingInfoLimitValue = "0";
        private readonly string _removedBookingInfoLimitValue = "1";
        private readonly string _dateFormat;

        public SoapRequestCreator(string dateFormat = "yyyy-MM-ddTHH:mm:ss.fffffff")
        {
            _dateFormat = dateFormat;
        }

        public HotelAvailNotifRQRequest CreateRequest(BookingInfoChangesForHotel hotelChanges)
        {
            HotelAvailNotifRQRequest request = new HotelAvailNotifRQRequest();
            List<AvailStatusMessageType> availStatusMessages = new List<AvailStatusMessageType>();

            request.Security = new SecurityHeaderType
            {
                Username = hotelChanges.Hotel.Login,
                Password = hotelChanges.Hotel.Password
            };
            request.OTA_HotelAvailNotifRQ = new OTA_HotelAvailNotifRQ
            {
                TimeStamp = DateTime.Now,
                AvailStatusMessages = new OTA_HotelAvailNotifRQAvailStatusMessages { HotelCode = hotelChanges.Hotel.HotelCode },
            };
            foreach (BookingInfoChangesForRoom roomChanges in hotelChanges.ChangesForRooms)
            {
                List<AvailStatusMessageType> availStatusMessagesForCurrRoom = GetAvailStatusMessagesForOneRoom(
                    roomChanges.BookingInfoChanges, 
                    roomChanges.Room.TLApiCode, 
                    _dateFormat);
                availStatusMessages.AddRange(availStatusMessagesForCurrRoom);
            }
            request.OTA_HotelAvailNotifRQ.AvailStatusMessages.AvailStatusMessage = availStatusMessages.ToArray();
            return request;
        }

        private List<AvailStatusMessageType> GetAvailStatusMessagesForOneRoom(
            BookingInfoChanges bookingInfoChanges, 
            string tlApiCode, 
            string dateFormat)
        {
            List<AvailStatusMessageType> availStatusMessages = new List<AvailStatusMessageType>();
            foreach (BookingInfo bookingInfo in bookingInfoChanges.AddedBookingInfo)
            {
                AvailStatusMessageType currMessage = new AvailStatusMessageType
                {
                    BookingLimit = _addedBookingInfoLimitValue,
                    StatusApplicationControl = new StatusApplicationControlType
                    {
                        Start = bookingInfo.StartBooking.ToString(dateFormat),
                        End = bookingInfo.EndBooking.ToString(dateFormat),
                        InvTypeCode = tlApiCode
                    }
                };
                availStatusMessages.Add(currMessage);
            }
            foreach (BookingInfo bookingInfo in bookingInfoChanges.RemovedBookingInfo)
            {
                AvailStatusMessageType currMessage = new AvailStatusMessageType
                {
                    BookingLimit = _removedBookingInfoLimitValue,
                    StatusApplicationControl = new StatusApplicationControlType
                    {
                        Start = bookingInfo.StartBooking.ToString(dateFormat),
                        End = bookingInfo.EndBooking.ToString(dateFormat),
                        InvTypeCode = tlApiCode
                    }
                };
                availStatusMessages.Add(currMessage);
            }
            return availStatusMessages;
        }
    }
}
