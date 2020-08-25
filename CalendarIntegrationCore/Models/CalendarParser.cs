using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace CalendarIntegrationCore.Models
{
    public class CalendarParser: ICalendarParser
    {
        private static List<string> _dateParsingFormats = new List<string>{ "yyyyMMdd", "yyyyMMddTHHmmssZ" };
        
        private enum ParserState
        {
            InEventBlock,
            OutEventBlock
        }
    
        public List<BookingInfo> ParseCalendar(string calendar, int roomId)
        {
            DateTime startDate = default;
            DateTime endDate = default;
            ParserState state = ParserState.OutEventBlock;
            List<string> calendarFileLines = calendar.Split('\n').Select(x => x.Trim()).ToList();
            List<BookingInfo> result = new List<BookingInfo>();

            for (int lineIndex = 0; lineIndex < calendarFileLines.Count; lineIndex++)
            {
                string currLine = calendarFileLines[lineIndex];
                if (currLine == "BEGIN:VEVENT")
                {
                    if (state == ParserState.InEventBlock)
                    {
                        throw new CalendarParserException($"Unexpected start of \"VEVENT\" block on line {lineIndex + 1}");
                    }
                    else
                    {
                        state = ParserState.InEventBlock;
                    }
                }
                else if (currLine == "END:VEVENT")
                {
                    if (state == ParserState.OutEventBlock)
                    {
                        throw new CalendarParserException($"Unexpected end of \"VEVENT\" block on line {lineIndex + 1}");
                    }
                    else if (startDate == default)
                    {
                        throw new CalendarParserException("One of the blocks doesn't contain a start date");
                    }
                    else if (endDate == default)
                    {
                        throw new CalendarParserException("One of the blocks doesn't contain an end date");
                    }
                    else
                    {
                        state = ParserState.OutEventBlock;
                        result.Add(new BookingInfo
                        {
                            RoomId = roomId,
                            StartBooking = startDate,
                            EndBooking = endDate
                        });
                        startDate = endDate = default;
                    }
                }
                else if (Regex.IsMatch(currLine, @"DTSTART.*"))
                {
                    startDate = ParseDate(currLine.Split(':').Last());
                }
                else if (Regex.IsMatch(currLine, @"DTEND.*"))
                {
                    endDate = ParseDate(currLine.Split(':').Last());
                }
            }
            return result.OrderBy(elem => elem.StartBooking).ToList();
        }
        
        public DateTime ParseDate(string dateInInitialFormat)
        {
            DateTime result;
            foreach (var currFormat in _dateParsingFormats)
            {
                if (DateTime.TryParseExact(dateInInitialFormat, currFormat,
                    CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out result))
                {
                    return result.ToUniversalTime();
                }
            }
            throw new CalendarParserException("Can't parse data using specified formats");
        }
    }
}