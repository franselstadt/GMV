using System.Threading;
using System.Threading.Tasks;
using Asp.Versioning;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using gmvTM.Application.Classes.Simulation;
using gmvTM.Application.Interfaces;
using gmvTM.Domain.Workers.Interfaces;

namespace gmvTM.Server.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/admin")]
    public sealed class AdminController : ControllerBase
    {
        private readonly IDataSeederWorker _seeder;
        private readonly ISimulationStore _simulations;
        private readonly ILogger<AdminController> _logger;

        public AdminController(IDataSeederWorker seeder, ISimulationStore simulations, ILogger<AdminController> logger)
        {
            _seeder = seeder;
            _simulations = simulations;
            _logger = logger;
        }

        [HttpPost("database/reseed")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> ReseedDatabase(CancellationToken ct)
        {
            _logger.LogInformation(gmvServer.Messages.LogReseedingDatabase);

            foreach (ActiveSimulation simulation in _simulations.ListActive())
                _simulations.TryRemove(simulation.ID, out _);

            await _seeder.ReseedAsync(ct);

            _logger.LogInformation(gmvServer.Messages.LogDatabaseReseeded);
            return NoContent();
        }
    }
}
