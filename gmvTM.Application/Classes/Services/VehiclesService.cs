using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using gmvTM.Application.Interfaces;
using gmvTM.Domain.Collections.Interfaces;
using gmvTM.Domain.Extensions.Collections;
using gmvTM.Domain.Items;

namespace gmvTM.Application.Classes.Services
{
    public sealed class VehiclesService : IVehiclesService
    {
        private readonly IVehicleCollection _vehicles;

        public VehiclesService(IVehicleCollection vehicles)
        {
            _vehicles = vehicles;
        }

        public async Task<IReadOnlyList<VehicleItem>> GetVehiclesAsync(CancellationToken ct)
        {
            return await _vehicles.GetAllOrderedAsync(ct);
        }
    }
}
