using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace CalendarIntegrationCore.Models
{
    public class CalendarParser: ICalendarParser
    {
        private List<string> _dateParsingFormats = new List<string>{ "yyyyMMdd", "yyyyMMddTHHmmssZ" };

        public CalendarParser() { }
        public CalendarParser(List<string> dateParsingFormats)
        {
            _dateParsingFormats = dateParsingFormats;
        }
        
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
            
            foreach (var (currLine, lineIndex) in calendarFileLines.Select((value, idx) => ( value, idx )))
            {
                if (currLine == "BEGIN:VEVENT")
                {
                    if (state == ParserState.InEventBlock)
                    {
                        throw new FormatException($"Unexpected start of \"VEVENT\" block on line {lineIndex + 1}");
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
                        throw new FormatException($"Unexpected end of \"VEVENT\" block on line {lineIndex + 1}");
                    }
                    else if (startDate == default)
                    {
                        throw new FormatException("One of the blocks doesn't contain a start date");
                    }
                    else if (endDate == default)
                    {
                        throw new FormatException("One of the blocks doesn't contain an end date");
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
            throw new FormatException("Can't parse data using specified formats");
        }
    }
}