using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using gmvTM.Domain.Collections.Interfaces;
using gmvTM.Domain.Items;

namespace gmvTM.Domain.Extensions.Collections
{
    public static class TripCollectionExtensions
    {
        public static IReadOnlyList<TripItem> GetByRouteID(this ITripCollection trips, int routeID)
        {
            ArgumentNullException.ThrowIfNull(trips);
            return trips.ReadByRouteID(routeID);
        }

        public static Task<IReadOnlyList<TripItem>> GetByRouteIDAsync(this ITripCollection trips, int routeID, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(trips);
            return trips.ReadByRouteIDAsync(routeID, cancellationToken);
        }

        public static IReadOnlyList<TripItem> GetActiveByRouteID(this ITripCollection trips, int routeID)
        {
            ArgumentNullException.ThrowIfNull(trips);
            return trips.ReadActiveByRouteID(routeID);
        }

        public static Task<IReadOnlyList<TripItem>> GetActiveByRouteIDAsync(this ITripCollection trips, int routeID, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(trips);
            return trips.ReadActiveByRouteIDAsync(routeID, cancellationToken);
        }
    }
}
