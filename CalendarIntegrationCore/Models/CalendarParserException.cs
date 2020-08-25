using System;
using System.Collections.Generic;
using System.Text;

namespace CalendarIntegrationCore.Models
{
    public class CalendarParserException: Exception
    {
        public CalendarParserException(): base() { }
        public CalendarParserException(string message) : base(message) { }
        public CalendarParserException(string message, Exception inner) : base(message, inner) { }
    }
}
