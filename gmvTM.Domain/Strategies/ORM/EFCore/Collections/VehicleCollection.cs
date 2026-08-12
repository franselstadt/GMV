using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using gmvTM.Domain.Collections.Interfaces;
using gmvTM.Domain.Strategies.ORM.EFCore.Infrastructure;
using gmvTM.Domain.Items;

namespace gmvTM.Domain.Strategies.ORM.EFCore.Collections
{
    public sealed class VehicleCollection : BaseCollection<VehicleItem>, IVehicleCollection
    {
        public VehicleCollection(DatabaseContext context) : base(context)
        {
        }

        public IReadOnlyList<VehicleItem> ReadAllOrdered()
        {
            return this.ReadItems()
                .OrderBy(v => v.FleetCode, StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        public async Task<IReadOnlyList<VehicleItem>> ReadAllOrderedAsync(CancellationToken cancellationToken = default)
        {
            IReadOnlyList<VehicleItem> all = await this.ReadItemsAsync(cancellationToken).ConfigureAwait(false);

            return all
                .OrderBy(v => v.FleetCode, StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        public VehicleItem? ReadFirst()
        {
            return this.ReadItems()
                .OrderBy(v => v.ID)
                .FirstOrDefault();
        }

        public async Task<VehicleItem?> ReadFirstAsync(CancellationToken cancellationToken = default)
        {
            IReadOnlyList<VehicleItem> all = await this.ReadItemsAsync(cancellationToken).ConfigureAwait(false);

            return all
                .OrderBy(v => v.ID)
                .FirstOrDefault();
        }

        public IReadOnlyList<VehicleItem> ReadByRouteID(ITripCollection trips, int routeID)
        {
            ArgumentNullException.ThrowIfNull(trips);

            HashSet<int> assignedVehicleIDs = trips.ReadByRouteID(routeID)
                .Select(t => t.VehicleID)
                .ToHashSet();

            return this.ReadAllOrdered()
                .Where(v => assignedVehicleIDs.Contains(v.ID))
                .ToList();
        }

        public async Task<IReadOnlyList<VehicleItem>> ReadByRouteIDAsync(ITripCollection trips, int routeID, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(trips);

            IReadOnlyList<TripItem> assignments = await trips.ReadByRouteIDAsync(routeID, cancellationToken).ConfigureAwait(false);
            HashSet<int> assignedVehicleIDs = assignments
                .Select(t => t.VehicleID)
                .ToHashSet();

            IReadOnlyList<VehicleItem> all = await this.ReadAllOrderedAsync(cancellationToken).ConfigureAwait(false);

            return all
                .Where(v => assignedVehicleIDs.Contains(v.ID))
                .ToList();
        }
    }
}
