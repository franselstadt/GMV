using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using gmvTM.Domain.Collections.Interfaces;
using gmvTM.Domain.Items;

namespace gmvTM.Domain.Extensions.Collections
{
    public static class VehicleCollectionExtensions
    {
        public static IReadOnlyList<VehicleItem> GetAllOrdered(this IVehicleCollection vehicles)
        {
            ArgumentNullException.ThrowIfNull(vehicles);

            return vehicles.ReadItems()
                .OrderBy(v => v.FleetCode, StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        public static async Task<IReadOnlyList<VehicleItem>> GetAllOrderedAsync(this IVehicleCollection vehicles, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(vehicles);

            IReadOnlyList<VehicleItem> all = await vehicles.ReadItemsAsync(cancellationToken)
                .ConfigureAwait(false);

            return all
                .OrderBy(v => v.FleetCode, StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        public static VehicleItem? GetFirst(this IVehicleCollection vehicles)
        {
            ArgumentNullException.ThrowIfNull(vehicles);
            return vehicles.ReadItems()
                .OrderBy(v => v.ID)
                .FirstOrDefault();
        }

        public static async Task<VehicleItem?> GetFirstAsync(this IVehicleCollection vehicles, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(vehicles);

            IReadOnlyList<VehicleItem> all = await vehicles.ReadItemsAsync(cancellationToken)
                .ConfigureAwait(false);

            return all
                .OrderBy(v => v.ID)
                .FirstOrDefault();
        }
    }
}
