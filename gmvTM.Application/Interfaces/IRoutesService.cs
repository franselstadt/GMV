using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using gmvTM.Domain;
using gmvTM.Domain.Items;

namespace gmvTM.Application.Interfaces
{
    public interface IRoutesService
    {
        public Task<IReadOnlyList<RouteItem>> GetRoutesAsync(CancellationToken ct);

        public Task<RouteItem> GetRouteByCodeAsync(string routeCode, CancellationToken ct);

        public Task<RouteShapeDto> GetRouteShapeByCodeAsync(string routeCode, CancellationToken ct);

        public Task<IReadOnlyList<VehicleItem>> GetVehiclesByRouteCodeAsync(string routeCode, CancellationToken ct);
    }
}
