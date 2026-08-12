using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using gmvTM.Domain;

namespace gmvTM.Application.Interfaces
{
    public interface ISimulationService
    {
        public Task<SimulationRunDto> StartAsync(string routeCode, StartSimulationRequest request, CancellationToken ct);

        public Task StopAsync(int simulationRunID, CancellationToken ct);

        public Task<IReadOnlyList<SimulationRunDto>> GetActiveAsync(CancellationToken ct);

        public Task TickAsync(CancellationToken ct);
    }
}
