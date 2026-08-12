using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using gmvTM.Domain.Items;

namespace gmvTM.Domain.Collections.Interfaces
{
    public interface IVehicleCollection : IBaseCollection<VehicleItem>
    {
        IReadOnlyList<VehicleItem> ReadAllOrdered();
        Task<IReadOnlyList<VehicleItem>> ReadAllOrderedAsync(CancellationToken cancellationToken = default);
        VehicleItem? ReadFirst();
        Task<VehicleItem?> ReadFirstAsync(CancellationToken cancellationToken = default);
        IReadOnlyList<VehicleItem> ReadByRouteID(ITripCollection trips, int routeID);
        Task<IReadOnlyList<VehicleItem>> ReadByRouteIDAsync(ITripCollection trips, int routeID, CancellationToken cancellationToken = default);
    }
}
