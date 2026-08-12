using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using gmvTM.Domain;
using gmvTM.Domain.Items;

namespace gmvTM.Application.Interfaces
{
    public interface IRouteStopService
    {
        public Task<PagedResult<StopItem>> GetStopsAsync(string routeCode, int page, int pageSize, CancellationToken ct);

        public Task<StopItem> GetStopByCodeAsync(string routeCode, string stopCode, CancellationToken ct);

        public Task<NextArrivalDto?> GetNextArrivalsAsync(string routeCode, string stopCode, CancellationToken ct);
    }
}
