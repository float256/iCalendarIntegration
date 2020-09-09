using System.Threading;

namespace CalendarIntegrationCore.Services.DataRetrieving
{
    public interface IAvailabilityInfoReceiver
    {
        string GetCalendarByUrl(string url, CancellationToken cancelToken);
    }
}
