using System;
using System.Collections.Generic;
using CalendarIntegrationCore.Models;
using TLConnect;

namespace CalendarIntegrationWeb.Services.DataUploading
{
    public class SoapRequestCreator: ISoapRequestCreator
    {
        private readonly string _dateFormat;
        private readonly Dictionary<BookingLimitType, string> _bookingLimitAsString = new Dictionary<BookingLimitType, string>
        {
            { BookingLimitType.Available, "1" },
            { BookingLimitType.Occupied, "0" }
        };
        
        public SoapRequestCreator(string dateFormat = "yyyy-MM-dd")
        {
            _dateFormat = dateFormat;
        }

        public HotelAvailNotifRQRequest CreateRequest(
            Dictionary<Room, List<AvailabilityStatusMessage>> availStatuses,
            string username,
            string password,
            string hotelCode)
        {
            HotelAvailNotifRQRequest request = new HotelAvailNotifRQRequest();
            List<AvailStatusMessageType> availStatusMessages = new List<AvailStatusMessageType>();

            request.Security = new SecurityHeaderType
            {
                Username = username,
                Password = password
            };
            request.OTA_HotelAvailNotifRQ = new OTA_HotelAvailNotifRQ
            {
                TimeStamp = DateTime.Now,
                AvailStatusMessages = new OTA_HotelAvailNotifRQAvailStatusMessages { HotelCode = hotelCode },
            };
            foreach (Room currRoom in availStatuses.Keys)
            {
                foreach (var currAvailStatus in availStatuses[currRoom])
                {
                    availStatusMessages.Add(new AvailStatusMessageType
                    {
                        BookingLimit = _bookingLimitAsString[currAvailStatus.State],
                        StatusApplicationControl = new StatusApplicationControlType
                        {
                            Start = currAvailStatus.StartDate.ToString(_dateFormat),
                            End = currAvailStatus.EndDate.ToString(_dateFormat),
                            InvTypeCode = currRoom.TLApiCode
                        }
                    });
                }
            }
            request.OTA_HotelAvailNotifRQ.AvailStatusMessages.AvailStatusMessage = availStatusMessages.ToArray();
            return request;
        }
    }
}
