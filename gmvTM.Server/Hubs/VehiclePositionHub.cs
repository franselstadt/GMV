using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.SignalR;
using gmvTM.Domain;

namespace gmvTM.Server.Hubs
{
    //generic track any vehcile hub for websocket, but we mock so really we only track the vehicles in the simulation
    //the vehicles to monitor come from the connection url: /hubs/vehicle/{fleetCodes} where fleetCodes is a comma separated list
    public sealed class VehiclePositionHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            HttpContext? http = Context.GetHttpContext();
            string? fleetCodes = http?.GetRouteValue(gmvDomain.AppConstants.VehiclePositionHubFleetCodesParam) as string;

            if (!string.IsNullOrWhiteSpace(fleetCodes))
            {
                foreach (string fleetCode in fleetCodes.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
                    await Groups.AddToGroupAsync(Context.ConnectionId, gmvDomain.AppConstants.VehicleGroupName(fleetCode));
            }

            await base.OnConnectedAsync();
        }
    }
}
