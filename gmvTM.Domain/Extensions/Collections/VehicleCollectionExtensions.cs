using System;
using System.Collections.Generic;
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
            return vehicles.ReadAllOrdered();
        }

        public static Task<IReadOnlyList<VehicleItem>> GetAllOrderedAsync(this IVehicleCollection vehicles, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(vehicles);
            return vehicles.ReadAllOrderedAsync(cancellationToken);
        }

        public static VehicleItem? GetFirst(this IVehicleCollection vehicles)
        {
            ArgumentNullException.ThrowIfNull(vehicles);
            return vehicles.ReadFirst();
        }

        public static Task<VehicleItem?> GetFirstAsync(this IVehicleCollection vehicles, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(vehicles);
            return vehicles.ReadFirstAsync(cancellationToken);
        }

        public static IReadOnlyList<VehicleItem> GetByRouteID(this IVehicleCollection vehicles, ITripCollection trips, int routeID)
        {
            ArgumentNullException.ThrowIfNull(vehicles);
            return vehicles.ReadByRouteID(trips, routeID);
        }

        public static Task<IReadOnlyList<VehicleItem>> GetByRouteIDAsync(this IVehicleCollection vehicles, ITripCollection trips, int routeID, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(vehicles);
            return vehicles.ReadByRouteIDAsync(trips, routeID, cancellationToken);
        }
    }
}
