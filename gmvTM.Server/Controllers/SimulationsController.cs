using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using gmvTM.Application.Interfaces;
using gmvTM.Domain;
using gmvTM.Server.Monitoring;

namespace gmvTM.Server.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/simulations")]
    [Produces(ContentTypes.ApplicationJson)]
    public sealed class SimulationsController : ControllerBase
    {
        private readonly ISimulationService _simulations;
        private readonly ILogger<SimulationsController> _logger;

        public SimulationsController(ISimulationService simulations, ILogger<SimulationsController> logger)
        {
            _simulations = simulations;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IReadOnlyList<SimulationRunDto>> GetActive(CancellationToken ct)
        {
            _logger.LogInformation(gmvServer.Messages.LogListingSimulations);
            IReadOnlyList<SimulationRunDto> active = await _simulations.GetActiveAsync(ct);
            _logger.LogInformation(gmvServer.Messages.LogReturningSimulations, active.Count);
            return active;
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Stop(int id, CancellationToken ct)
        {
            _logger.LogInformation(gmvServer.Messages.LogStoppingSimulation, id);
            await _simulations.StopAsync(id, ct);
            AppMetrics.SimulationsStopped.Inc();
            _logger.LogInformation(gmvServer.Messages.LogSimulationStopped, id);
            return NoContent();
        }
    }
}
