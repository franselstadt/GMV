using System;
using System.Collections.Generic;
using System.Linq;
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

            return trips.ReadItems()
                .Where(t => t.RouteID == routeID)
                .OrderByDescending(t => t.StartedAtUtc)
                .ToList();
        }

        public static async Task<IReadOnlyList<TripItem>> GetByRouteIDAsync(this ITripCollection trips, int routeID, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(trips);

            IReadOnlyList<TripItem> all = await trips.ReadItemsAsync(cancellationToken).ConfigureAwait(false);


            return all
                .Where(t => t.RouteID == routeID)
                .OrderByDescending(t => t.StartedAtUtc)
                .ToList();
        }

        public static IReadOnlyList<TripItem> GetActiveByRouteID(this ITripCollection trips, int routeID)
        {
            ArgumentNullException.ThrowIfNull(trips);


            return trips.GetByRouteID(routeID)
                .Where(t => string.Equals(t.Status, Messages.SimulationStatusRunning, StringComparison.Ordinal))
                .ToList();
        }

        public static async Task<IReadOnlyList<TripItem>> GetActiveByRouteIDAsync(this ITripCollection trips, int routeID, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(trips);


            IReadOnlyList<TripItem> rows = await trips.GetByRouteIDAsync(routeID, cancellationToken)
                .ConfigureAwait(false);

            return rows
                .Where(t => string.Equals(t.Status, Messages.SimulationStatusRunning, StringComparison.Ordinal))
                .ToList();
        }
    }
}
