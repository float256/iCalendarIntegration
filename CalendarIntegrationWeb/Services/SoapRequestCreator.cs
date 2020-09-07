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
        private readonly string _dateFormat;
        private readonly Dictionary<BookingLimitType, string> _bookingLimitAsString = new Dictionary<BookingLimitType, string>
        {
            { BookingLimitType.Available, "1" },
            { BookingLimitType.Occupied, "0" }
        };
        
        public SoapRequestCreator(string dateFormat = "yyyy-MM-ddTHH:mm:ss.fffffff")
        {
            _dateFormat = dateFormat;
        }

        public HotelAvailNotifRQRequest CreateRequest(List<AvailabilityStatusMessage> availStatuses,
            string username, string password, string hotelCode)
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
            foreach (AvailabilityStatusMessage currAvailStatus in availStatuses)
            {
                availStatusMessages.Add(new AvailStatusMessageType
                {
                    BookingLimit = _bookingLimitAsString[currAvailStatus.State],
                    StatusApplicationControl = new StatusApplicationControlType
                    {
                        Start = currAvailStatus.StartDate.ToString(_dateFormat),
                        End = currAvailStatus.EndDate.ToString(_dateFormat),
                        InvTypeCode = currAvailStatus.TLApiCode
                    }
                });
            }
            request.OTA_HotelAvailNotifRQ.AvailStatusMessages.AvailStatusMessage = availStatusMessages.ToArray();
            return request;
        }
    }
}
