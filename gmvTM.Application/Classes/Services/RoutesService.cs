using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using gmvTM.Application.Classes.Tools;
using gmvTM.Application.Interfaces;
using gmvTM.Domain;
using gmvTM.Domain.Collections.Interfaces;
using gmvTM.Domain.Extensions.Collections;
using gmvTM.Domain.Items;
using gmvTM.Domain.Workers.Interfaces;

namespace gmvTM.Application.Classes.Services
{
    public sealed class RoutesService : IRoutesService
    {
        private readonly IRouteCollection _routes;
        private readonly IVehicleCollection _vehicles;
        private readonly ITripCollection _trips;
        private readonly IPolylineDecoderWorker _polylineDecoder;

        public RoutesService(IRouteCollection routes, IVehicleCollection vehicles, ITripCollection trips, IPolylineDecoderWorker polylineDecoder)
        {
            _routes = routes;
            _vehicles = vehicles;
            _trips = trips;
            _polylineDecoder = polylineDecoder;
        }

        public async Task<IReadOnlyList<RouteItem>> GetRoutesAsync(CancellationToken ct)
        {
            return await _routes.GetAllOrderedByShortNameAsync(ct);
        }

        public Task<RouteItem> GetRouteByCodeAsync(string routeCode, CancellationToken ct)
        {
            return RouteCodeLookup.RequireAsync(_routes, routeCode, ct);
        }

        public async Task<RouteShapeDto> GetRouteShapeByCodeAsync(string routeCode, CancellationToken ct)
        {
            RouteItem route = await RouteCodeLookup.RequireAsync(_routes, routeCode, ct);

            return ItemFactory.CreateItem<RouteShapeDto>(new
            {
                Points = _polylineDecoder.Decode(route.EncodedPolyline)
            });
        }

        public async Task<IReadOnlyList<VehicleItem>> GetVehiclesByRouteCodeAsync(string routeCode, CancellationToken ct)
        {
            RouteItem route = await RouteCodeLookup.RequireAsync(_routes, routeCode, ct);
            return await _vehicles.GetByRouteIDAsync(_trips, route.ID, ct);
        }
    }
}
