using System;

namespace CalendarIntegrationCore.Services.DataProcessing
{
    public class AvailabilityInfoDataProcessorException: Exception
    {
        public AvailabilityInfoDataProcessorException(): base() { }
        public AvailabilityInfoDataProcessorException(string message) : base(message) { }
        public AvailabilityInfoDataProcessorException(string message, Exception inner) : base(message, inner) { }
    }
}