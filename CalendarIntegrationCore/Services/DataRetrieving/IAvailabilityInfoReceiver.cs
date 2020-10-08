using System.Threading;
using System.Threading.Tasks;

namespace CalendarIntegrationCore.Services.DataRetrieving
{
    public interface IAvailabilityInfoReceiver
    {
        Task<string> GetCalendarByUrl(string url, CancellationToken cancelToken);
    }
}
