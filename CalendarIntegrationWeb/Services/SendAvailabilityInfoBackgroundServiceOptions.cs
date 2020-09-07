namespace CalendarIntegrationWeb.Services
{
    public class SendAvailabilityInfoBackgroundServiceOptions
    {
        public int SendingPeriodInSeconds { get; set; }
        public int DataPackageSize { get; set; }
    }
}