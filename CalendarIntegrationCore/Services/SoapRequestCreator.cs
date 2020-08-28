using CalendarIntegrationCore.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace CalendarIntegrationCore.Services
{
    class SoapRequestCreator
    {
        private XmlDocument _document = new XmlDocument();
        private XmlNode _nodeWithAllBookingInfo;
        private string _hotelAvailNotifRqNodeDateFormat = "yyyy-MM-ddTHH:mm:ss.fffffff";
        private string _statusApplicationControlDateFormat = "yyyy-MM-ddTHH:mm:ss";

        public string QueryText { get => _document.OuterXml; }

        public SoapRequestCreator(string username, string password, string hotelCode)
        {
            CreateQueryMainElements(username, password, hotelCode);
        }

        private void CreateQueryMainElements(string username, string password, string hotelCode)
        {
            XmlNode envelopeNode = _document.CreateElement("s", "Envelope", "http://schemas.xmlsoap.org/soap/envelope/");
            _document.AppendChild(envelopeNode);

            XmlNode headerNode = _document.CreateElement("s", "Header", "http://schemas.xmlsoap.org/soap/envelope/");
            envelopeNode.AppendChild(headerNode);

            XmlNode securityNode = _document.CreateElement("h", "Security", "https://www.travelline.ru/Api/TLConnect");
            securityNode.Attributes.Append(_document.CreateAttribute("Username")).Value = username;
            securityNode.Attributes.Append(_document.CreateAttribute("Password")).Value = password;
            securityNode.Attributes.Append(_document.CreateAttribute("xmlns")).Value = "https://www.travelline.ru/Api/TLConnect";
            securityNode.Attributes.Append(_document.CreateAttribute("xmlns:xsd")).Value = "http://www.w3.org/2001/XMLSchema";
            securityNode.Attributes.Append(_document.CreateAttribute("xmlns:xsi")).Value = "http://www.w3.org/2001/XMLSchema-instance";
            headerNode.AppendChild(securityNode);

            XmlNode bodyNode = _document.CreateElement("s", "Body", "http://schemas.xmlsoap.org/soap/envelope/");
            bodyNode.Attributes.Append(_document.CreateAttribute("xmlns:xsi")).Value = "http://www.w3.org/2001/XMLSchema-instance";
            bodyNode.Attributes.Append(_document.CreateAttribute("xmlns:xsd")).Value = "http://www.w3.org/2001/XMLSchema";
            envelopeNode.AppendChild(bodyNode);

            XmlNode hotelAvailNotifRqNode = _document.CreateElement("OTA_HotelAvailNotifRQ");
            hotelAvailNotifRqNode.Attributes.Append(_document.CreateAttribute("TimeStamp")).Value = DateTime.Now.ToString(_hotelAvailNotifRqNodeDateFormat);
            hotelAvailNotifRqNode.Attributes.Append(_document.CreateAttribute("Version")).Value = "1.6";
            hotelAvailNotifRqNode.Attributes.Append(_document.CreateAttribute("xmlns")).Value = "http://www.opentravel.org/OTA/2003/05";
            bodyNode.AppendChild(hotelAvailNotifRqNode);

            XmlNode availStatusMessagesNode = _document.CreateElement("AvailStatusMessages");
            availStatusMessagesNode.Attributes.Append(_document.CreateAttribute("HotelCode")).Value = hotelCode;
            hotelAvailNotifRqNode.AppendChild(availStatusMessagesNode);
            _nodeWithAllBookingInfo = availStatusMessagesNode;
        }

        public void AddBookingInfo(BookingInfo bookingInfo, string tlApiCode)
        {
            XmlNode availStatusMessageNode = _document.CreateElement("AvailStatusMessage");
            availStatusMessageNode.Attributes.Append(_document.CreateAttribute("BookingLimit")).Value = "1";

            XmlNode statusApplicationControlNode = _document.CreateElement("StatusApplicationControl");
            statusApplicationControlNode.Attributes.Append(_document.CreateAttribute("Start")).Value = bookingInfo.StartBooking
                .ToString(_statusApplicationControlDateFormat);
            statusApplicationControlNode.Attributes.Append(_document.CreateAttribute("End")).Value = bookingInfo.EndBooking
                .ToString(_statusApplicationControlDateFormat);
            statusApplicationControlNode.Attributes.Append(_document.CreateAttribute("InvTypeCode")).Value = tlApiCode;
            _nodeWithAllBookingInfo.AppendChild(statusApplicationControlNode);
        }
    }
}
