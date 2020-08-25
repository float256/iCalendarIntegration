using System;
using System.Collections.Generic;
using CalendarIntegrationCore.Models;
using Xunit;
using Moq;

namespace CalendarIntegrationCore.Tests
{
    public class CalendarParserTests
    {
        [Fact]
        public void CalendarParser_ParseCalendar_EmptyTemplate_EmptyList()
        {
            // Arrange
            int roomId = 1;
            string template = String.Join(Environment.NewLine,
                "BEGIN:VCALENDAR",
                "VERSION:2.0",
                "CALSCALE:GREGORIAN",
                "METHOD:PUBLISH",
                "END:VCALENDAR");
            CalendarParser calendarParser = new CalendarParser();
            
            // Act
            List<BookingInfo> actual = calendarParser.ParseCalendar(template, roomId);
            
            // Assert
            Assert.Empty(actual);
        }
        
        [Fact]
        public void CalendarParser_ParseCalendar_CorrectTemplate_DatesFromTemplate()
        {
            // Arrange
            int roomId = 1;
            string template = String.Join(Environment.NewLine,
                "BEGIN:VCALENDAR",
                "VERSION:2.0",
                "CALSCALE:GREGORIAN\n",

                "BEGIN:VEVENT",
                "DTSTAMP:20200819T200836Z",
                "DTSTART;VALUE=DATE:20200829",
                "DTEND;VALUE=DATE:20200830",
                "SUMMARY:Not available",
                "END:VEVENT\n",

                "BEGIN:VEVENT",
                "DTSTAMP:20200819T200836Z",
                "DTSTART;VALUE=DATE:20210630T101826Z",
                "DTEND;VALUE=DATE:20210820T210856Z",
                "SUMMARY:Not available",
                "END:VEVENT\n",
                
                "BEGIN:VEVENT",
                "DTSTAMP:20200819T200836Z",
                "DTSTART;VALUE=DATE:20200926",
                "DTEND;VALUE=DATE:20201024",
                "SUMMARY:Not available",
                "END:VEVENT\n",

                "END:VCALENDAR"
            );
            List<BookingInfo> expectedDates = new List<BookingInfo>
            {
                new BookingInfo
                {
                    Id = roomId, 
                    StartBooking = new DateTime(2020, 08,29),
                    EndBooking = new DateTime(2020, 08, 30)
                },
                new BookingInfo
                {
                    Id = roomId, 
                    StartBooking = new DateTime(2020, 09, 26),
                    EndBooking = new DateTime(2020, 10, 24)
                },
                new BookingInfo
                {
                    Id = roomId,
                    StartBooking = new DateTime(2021, 06, 30, 10, 18, 26),
                    EndBooking = new DateTime(2021, 08, 20, 21, 08, 56)
                }
            };
            CalendarParser calendarParser = new CalendarParser();
            
            
            // Act
            List<BookingInfo> parsedDatesFromTemplate = calendarParser.ParseCalendar(template, roomId);
            
            // Assert
            for (int i = 0; i < expectedDates.Count; i++)
            {
                BookingInfo currExpectedDate = expectedDates[i];
                BookingInfo currDateFromTemplate = parsedDatesFromTemplate[i];
                
                Assert.Equal(currExpectedDate, currDateFromTemplate);
            }
        }

        [Fact]
        public void CalendarParser_ParseCalendar_UnexpectedBeginLine_ThrowsFormatException()
        {
            // Arrange
            string incorrectTemplate = String.Join(Environment.NewLine,
                "BEGIN:VCALENDAR",
                "VERSION:2.0",
                "CALSCALE:GREGORIAN",
                "METHOD:PUBLISH",

                "BEGIN:VEVENT",
                "BEGIN:VEVENT",
                "DTEND;VALUE=DATE:20210820",
                "DTSTART;VALUE=DATE:20210819",
                "SUMMARY:Not available",
                "END:VEVENT",

                "END:VCALENDAR");
            int roomId = 1;
            CalendarParser parser = new CalendarParser();
            string expectedExceptionMessage = "Unexpected start of \"VEVENT\" block on line 6";
            
            // Act
            Action act = () => parser.ParseCalendar(incorrectTemplate, roomId);
            
            //Assert
            FormatException exception = Assert.Throws<FormatException>(act);
            Assert.Equal(expectedExceptionMessage, exception.Message);
        }
        
        [Fact]
        public void CalendarParser_ParseCalendar_UnexpectedEndLine_ThrowsFormatException()
        {
            // Arrange
            string incorrectTemplate = String.Join(Environment.NewLine,
                "BEGIN:VCALENDAR",
                "VERSION:2.0",
                "CALSCALE:GREGORIAN",
                "METHOD:PUBLISH",

                "BEGIN:VEVENT",
                "DTEND;VALUE=DATE:19120520",
                "DTSTART;VALUE=DATE:19180819",
                "SUMMARY:Not available",
                "END:VEVENT",
                "END:VEVENT",

                "END:VCALENDAR");
            int roomId = 1;
            CalendarParser parser = new CalendarParser();
            string expectedExceptionMessage = "Unexpected end of \"VEVENT\" block on line 10";
            
            // Act
            Action act = () => parser.ParseCalendar(incorrectTemplate, roomId);
            
            //Assert
            FormatException exception = Assert.Throws<FormatException>(act);
            Assert.Equal(expectedExceptionMessage, exception.Message);
        }
        
        [Fact]
        public void CalendarParser_ParseCalendar_BlockWithoutStartDate_ThrowsFormatException()
        {
            // Arrange
            string incorrectTemplate = String.Join(Environment.NewLine,
                "BEGIN:VCALENDAR",
                "VERSION:2.0",
                "CALSCALE:GREGORIAN",
                "METHOD:PUBLISH",

                "BEGIN:VEVENT",
                "DTEND;VALUE=DATE:20201021",
                "SUMMARY:Not available",
                "END:VEVENT",

                "END:VCALENDAR");
            int roomId = 1;
            CalendarParser parser = new CalendarParser();
            string expectedExceptionMessage = "One of the blocks doesn't contain a start date";
            
            // Act
            Action act = () => parser.ParseCalendar(incorrectTemplate, roomId);
            
            //Assert
            FormatException exception = Assert.Throws<FormatException>(act);
            Assert.Equal(expectedExceptionMessage, exception.Message);
        }
        
        [Fact]
        public void CalendarParser_ParseCalendar_BlockWithoutEndDate_ThrowsFormatException()
        {
            // Arrange
            string incorrectTemplate = String.Join(Environment.NewLine,
                "BEGIN:VCALENDAR",
                "VERSION:2.0",
                "CALSCALE:GREGORIAN",
                "METHOD:PUBLISH",

                "BEGIN:VEVENT",
                "DTSTART;VALUE=DATE:19180819",
                "SUMMARY:Not available",
                "END:VEVENT",

                "END:VCALENDAR");
            int roomId = 1;
            CalendarParser parser = new CalendarParser();
            string expectedExceptionMessage = "One of the blocks doesn't contain an end date";
            
            // Act
            Action act = () => parser.ParseCalendar(incorrectTemplate, roomId);
            
            //Assert
            FormatException exception = Assert.Throws<FormatException>(act);
            Assert.Equal(expectedExceptionMessage, exception.Message);
        }
        
        [Fact]
        public void CalendarParser_ParseDate_DateWithoutTime_DateTimeObject()
        {
            // Arrange
            string dateString = "20200926";
            DateTime expectedDate = new DateTime(2020, 09, 26);
            CalendarParser calendarParser = new CalendarParser();
            
            // Act
            DateTime actualDate = calendarParser.ParseDate(dateString);
            
            // Assert
            Assert.Equal(expectedDate, actualDate);
        }
        
        [Fact]
        public void CalendarParser_ParseDate_DateWithTime_DateTimeObject()
        {
            // Arrange
            string dateString = "19531219T123556Z";
            DateTime expectedDate = new DateTime(1953, 12, 19, 12, 35, 56, 
                DateTimeKind.Utc);
            CalendarParser calendarParser = new CalendarParser();
            
            // Act
            DateTime actualDate = calendarParser.ParseDate(dateString);
            
            // Assert
            Assert.Equal(expectedDate, actualDate);
        }

        [Fact]
        public void CalendarParser_ParseDate_IncorrectDateFormat_ThrowsFormatException()
        {
            // Arrange
            string incorrectDateString = "1231T23232214Z";
            CalendarParser calendarParser = new CalendarParser();
            string expectedExceptionMessage = "Can't parse data using specified formats";
            
            // Act
            Action act = () => calendarParser.ParseDate(incorrectDateString);
            
            //Assert
            FormatException exception = Assert.Throws<FormatException>(act);
            Assert.Equal(expectedExceptionMessage, exception.Message);
        }
    }
}