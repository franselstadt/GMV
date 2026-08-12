using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using gmvTM.Domain.Items;

namespace gmvTM.Domain.Collections.Interfaces
{
    public interface ITripCollection : IBaseCollection<TripItem>
    {
        IReadOnlyList<TripItem> ReadByRouteID(int routeID);
        Task<IReadOnlyList<TripItem>> ReadByRouteIDAsync(int routeID, CancellationToken cancellationToken = default);
        IReadOnlyList<TripItem> ReadActiveByRouteID(int routeID);
        Task<IReadOnlyList<TripItem>> ReadActiveByRouteIDAsync(int routeID, CancellationToken cancellationToken = default);
    }
}
