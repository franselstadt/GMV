using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using gmvTM.Application.Interfaces;
using gmvTM.Domain;
using gmvTM.Server.Hubs;

namespace gmvTM.Server.Realtime
{
    public sealed class SignalRVehiclePositionBroadcaster : IVehiclePositionBroadcaster
    {
        private readonly IHubContext<VehiclePositionHub> _hub;

        public SignalRVehiclePositionBroadcaster(IHubContext<VehiclePositionHub> hub) => _hub = hub;

        public Task BroadcastAsync(VehiclePositionDto position, CancellationToken ct) =>
            _hub.Clients.Group(gmvDomain.AppConstants.VehicleGroupName(position.VehicleNumber))
                .SendAsync(gmvDomain.AppConstants.VehiclePositionEvent, position, ct);
    }
}
