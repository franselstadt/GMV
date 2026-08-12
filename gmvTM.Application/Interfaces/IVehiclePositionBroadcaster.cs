using System.Threading;
using System.Threading.Tasks;
using gmvTM.Domain;

namespace gmvTM.Application.Interfaces
{
    public interface IVehiclePositionBroadcaster
    {
        public Task BroadcastAsync(VehiclePositionDto position, CancellationToken ct);
    }
}
