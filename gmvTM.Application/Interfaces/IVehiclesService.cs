using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using gmvTM.Domain.Items;

namespace gmvTM.Application.Interfaces
{
    public interface IVehiclesService
    {
        public Task<IReadOnlyList<VehicleItem>> GetVehiclesAsync(CancellationToken ct);
    }
}
