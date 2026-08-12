using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Asp.Versioning;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using gmvTM.Application.Interfaces;
using gmvTM.Domain;
using gmvTM.Domain.Items;

namespace gmvTM.Server.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/vehicles")]
    [Produces(ContentTypes.ApplicationJson)]
    public sealed class VehiclesController : ControllerBase
    {
        private readonly IVehiclesService _vehicles;
        private readonly ILogger<VehiclesController> _logger;

        public VehiclesController(IVehiclesService vehicles, ILogger<VehiclesController> logger)
        {
            _vehicles = vehicles;
            _logger = logger;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IReadOnlyList<VehicleItem>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IReadOnlyList<VehicleItem>>> GetVehicles(CancellationToken ct)
        {
            _logger.LogInformation(gmvServer.Messages.LogListingFleet);
            IReadOnlyList<VehicleItem> vehicles = await _vehicles.GetVehiclesAsync(ct);
            _logger.LogInformation(gmvServer.Messages.LogReturningFleet, vehicles.Count);
            return Ok(vehicles);
        }
    }
}
