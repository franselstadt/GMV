using System;
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
using gmvTM.Server.Monitoring;

namespace gmvTM.Server.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/routes")]
    [Produces(ContentTypes.ApplicationJson)]
    public sealed class RoutesController : ControllerBase
    {
        private readonly IRoutesService _routes;
        private readonly IRouteStopService _stops;
        private readonly ISimulationService _simulations;
        private readonly ILogger<RoutesController> _logger;

        public RoutesController(IRoutesService routes, IRouteStopService stops, ISimulationService simulations, ILogger<RoutesController> logger)
        {
            _routes = routes;
            _stops = stops;
            _simulations = simulations;
            _logger = logger;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IReadOnlyList<RouteItem>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IReadOnlyList<RouteItem>>> GetRoutes(CancellationToken ct)
        {
            _logger.LogInformation(gmvServer.Messages.LogListingRoutes);
            IReadOnlyList<RouteItem> routes = await _routes.GetRoutesAsync(ct);
            _logger.LogInformation(gmvServer.Messages.LogReturningRoutes, routes.Count);
            return Ok(routes);
        }

        [HttpGet("{routeCode}")]
        [ProducesResponseType(typeof(RouteItem), StatusCodes.Status200OK)]
        public async Task<ActionResult<RouteItem>> GetByCode(string routeCode, CancellationToken ct)
        {
            _logger.LogInformation(gmvServer.Messages.LogGettingRoute, routeCode);
            return Ok(await _routes.GetRouteByCodeAsync(routeCode, ct));
        }

        [HttpGet("{routeCode}/shape")]
        [ProducesResponseType(typeof(RouteShapeDto), StatusCodes.Status200OK)]
        public async Task<ActionResult<RouteShapeDto>> GetShape(string routeCode, CancellationToken ct)
        {
            _logger.LogInformation(gmvServer.Messages.LogGettingRouteShape, routeCode);
            RouteShapeDto shape = await _routes.GetRouteShapeByCodeAsync(routeCode, ct);
            _logger.LogDebug(gmvServer.Messages.LogRouteShapePoints, routeCode, shape.Points.Count);
            return Ok(shape);
        }

        [HttpGet("{routeCode}/vehicles")]
        [ProducesResponseType(typeof(IReadOnlyList<VehicleItem>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IReadOnlyList<VehicleItem>>> GetVehicles(string routeCode, CancellationToken ct)
        {
            _logger.LogInformation(gmvServer.Messages.LogListingVehicles, routeCode);
            return Ok(await _routes.GetVehiclesByRouteCodeAsync(routeCode, ct));
        }

        [HttpGet("{routeCode}/stops")]
        [ProducesResponseType(typeof(PagedResult<StopItem>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PagedResult<StopItem>>> GetStops(string routeCode, [FromQuery] int page = 1, [FromQuery] int pageSize = 50, CancellationToken ct = default)
        {
            _logger.LogInformation(gmvServer.Messages.LogListingStops, routeCode, page, pageSize);
            PagedResult<StopItem> stops = await _stops.GetStopsAsync(routeCode, page, pageSize, ct);
            _logger.LogInformation(gmvServer.Messages.LogReturningStops, stops.Items.Count, stops.TotalCount, routeCode);
            return Ok(stops);
        }

        [HttpGet("{routeCode}/stops/{stopCode}")]
        [ProducesResponseType(typeof(StopItem), StatusCodes.Status200OK)]
        public async Task<ActionResult<StopItem>> GetStop(string routeCode, string stopCode, CancellationToken ct)
        {
            _logger.LogInformation(gmvServer.Messages.LogGettingStop, stopCode, routeCode);
            return Ok(await _stops.GetStopByCodeAsync(routeCode, stopCode, ct));
        }

        [HttpGet("{routeCode}/stops/{stopCode}/arrivals/next")]
        [ProducesResponseType(typeof(NextArrivalDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<NextArrivalDto>> GetNextArrivals(string routeCode, string stopCode, CancellationToken ct = default)
        {
            _logger.LogInformation(gmvServer.Messages.LogCalculatingNextArrival, stopCode, routeCode);
            AppMetrics.NextArrivalRequests.WithLabels(routeCode).Inc();

            NextArrivalDto? arrival = await _stops.GetNextArrivalsAsync(routeCode, stopCode, ct);
            if (arrival is null)
            {
                _logger.LogInformation(gmvServer.Messages.LogNoUpcomingArrival, stopCode, routeCode);
                return NotFound();
            }

            _logger.LogInformation(
                gmvServer.Messages.LogNextArrival,
                stopCode,
                routeCode,
                arrival.RunLabel,
                arrival.PlannedTime,
                arrival.Status);
            return Ok(arrival);
        }

        [HttpPost("{routeCode}/simulations")]
        [ProducesResponseType(typeof(SimulationRunDto), StatusCodes.Status200OK)]
        public async Task<ActionResult<SimulationRunDto>> StartSimulation(string routeCode, [FromBody] StartSimulationRequest? request, CancellationToken ct)
        {
            StartSimulationRequest effective = request ?? new StartSimulationRequest();
            _logger.LogInformation(
                gmvServer.Messages.LogStartingSimulation,
                routeCode,
                effective.StopCode,
                effective.AverageMph,
                effective.AverageDwellSeconds);

            SimulationRunDto run = await _simulations.StartAsync(routeCode, effective, ct);
            AppMetrics.SimulationsStarted.WithLabels(run.RouteCode).Inc();

            _logger.LogInformation(
                gmvServer.Messages.LogSimulationStarted,
                run.ID,
                run.RouteCode,
                run.VehicleNumber,
                run.TripID);
            return Ok(run);
        }
    }
}
