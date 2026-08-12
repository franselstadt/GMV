using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using gmvTM.Domain.Collections.Base;
using gmvTM.Domain.Collections.Interfaces;
using gmvTM.Domain.Infrastructure.Persistence;
using gmvTM.Domain.Items;

namespace gmvTM.Domain.Collections
{
    public sealed class TripCollection : BaseCollection<TripItem>, ITripCollection
    {
        public TripCollection(DatabaseContext context) : base(context)
        {
        }

        public IReadOnlyList<TripItem> ReadByRouteID(int routeID)
        {
            return this.ReadItems()
                .Where(t => t.RouteID == routeID)
                .OrderByDescending(t => t.StartedAtUtc)
                .ToList();
        }

        public async Task<IReadOnlyList<TripItem>> ReadByRouteIDAsync(int routeID, CancellationToken cancellationToken = default)
        {
            IReadOnlyList<TripItem> all = await this.ReadItemsAsync(cancellationToken).ConfigureAwait(false);

            return all
                .Where(t => t.RouteID == routeID)
                .OrderByDescending(t => t.StartedAtUtc)
                .ToList();
        }

        public IReadOnlyList<TripItem> ReadActiveByRouteID(int routeID)
        {
            return this.ReadByRouteID(routeID)
                .Where(t => string.Equals(t.Status, Messages.SimulationStatusRunning, StringComparison.Ordinal))
                .ToList();
        }

        public async Task<IReadOnlyList<TripItem>> ReadActiveByRouteIDAsync(int routeID, CancellationToken cancellationToken = default)
        {
            IReadOnlyList<TripItem> rows = await this.ReadByRouteIDAsync(routeID, cancellationToken).ConfigureAwait(false);

            return rows
                .Where(t => string.Equals(t.Status, Messages.SimulationStatusRunning, StringComparison.Ordinal))
                .ToList();
        }
    }
}
