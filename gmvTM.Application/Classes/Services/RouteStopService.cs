using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using gmvTM.Application.Classes.Exceptions;
using gmvTM.Application.Classes.Simulation;
using gmvTM.Application.Classes.Tools;
using gmvTM.Application.Interfaces;
using gmvTM.Domain;
using gmvTM.Domain.Collections.Interfaces;
using gmvTM.Domain.Extensions.Collections;
using gmvTM.Domain.Extensions.Items;
using gmvTM.Domain.Items;
using gmvTM.Domain.Items.View;
using gmvTM.Domain.Workers.Interfaces;

namespace gmvTM.Application.Classes.Services
{
    public sealed class RouteStopService : IRouteStopService
    {
        private readonly IRouteCollection _routes;
        private readonly IStopCollection _stops;
        private readonly IStopPlanCollection _stopPlans;
        private readonly IDateTimeProviderWorker _clock;
        private readonly ITripPositionCalculatorWorker _positionCalculator;
        private readonly ISimulationStore _simulationStore;

        public RouteStopService(IRouteCollection routes, IStopCollection stops, IStopPlanCollection stopPlans, IDateTimeProviderWorker clock, ITripPositionCalculatorWorker positionCalculator, ISimulationStore simulationStore)
        {
            _routes = routes;
            _stops = stops;
            _stopPlans = stopPlans;
            _clock = clock;
            _positionCalculator = positionCalculator;
            _simulationStore = simulationStore;
        }

        public async Task<PagedResult<StopItem>> GetStopsAsync(string routeCode, int page, int pageSize, CancellationToken ct)
        {
            if (page < 1)
                throw new ValidationException(gmvDomain.Messages.PageMustBePositive);

            if (pageSize < 1)
                throw new ValidationException(gmvDomain.Messages.PageSizeMustBePositive);

            RouteItem route = await RouteCodeLookup.RequireAsync(_routes, routeCode, ct);
            IReadOnlyList<StopItem> allOnRoute = await _stops.GetByRouteIDAsync(route.ID, ct);

            return ItemFactory.CreateItem<PagedResult<StopItem>>(new
            {
                Items = allOnRoute
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList(),
                Page = page,
                PageSize = pageSize,
                TotalCount = allOnRoute.Count
            });
        }

        public async Task<StopItem> GetStopByCodeAsync(string routeCode, string stopCode, CancellationToken ct)
        {
            RouteItem route = await RouteCodeLookup.RequireAsync(_routes, routeCode, ct);
            string code = RouteStopTools.RequireStopCode(stopCode);
            StopItem? stop = await _stops.GetByCodeAsync(code, ct);

            if (stop is null || stop.RouteID != route.ID)
                throw new NotFoundException(string.Format(gmvDomain.Messages.StopNotFound, code, route.ShortName));

            return stop;
        }

        //claude assisted me with this method
        public async Task<NextArrivalDto?> GetNextArrivalsAsync(string routeCode, string stopCode, CancellationToken ct)
        {
            RouteItem route = await RouteCodeLookup.RequireAsync(_routes, routeCode, ct);
            string code = RouteStopTools.RequireStopCode(stopCode);

            ActiveSimulation? simulation = _simulationStore.ListActive()
                .FirstOrDefault(s =>
                    string.Equals(s.RouteCode, route.ShortName, StringComparison.OrdinalIgnoreCase));

            if (simulation is null)
                throw new ValidationException(gmvDomain.Messages.SimulationRequiredForArrivals);

            StopItem? stop = await _stops.GetByCodeAsync(code, ct);

            if (stop is null || stop.RouteID != route.ID)
                throw new NotFoundException(string.Format(gmvDomain.Messages.StopNotFound, code, route.ShortName));

            IReadOnlyList<StopPlanItem> schedule = await _stopPlans.GetByRouteIDAsync(_stops, route.ID, ct);

            int targetPathIndex = RouteStopTools.IndexOfStopOnOrAfter(
                simulation.Stops,
                simulation.StartStopIndex,
                code);

            if (targetPathIndex < 0)
                return null;

            PathStopViewItem targetPathStop = simulation.Stops[targetPathIndex];
            StopPlanItem? plannedStop = schedule
                .OrderBy(s => s.Sequence)
                .FirstOrDefault(s => s.StopID == targetPathStop.StopID);

            if (plannedStop is null)
                return null;

            int lastIndex = simulation.Stops.Count - 1;

            int plannedSeconds = targetPathIndex > simulation.StartStopIndex
                ? StopPlanItemExtensions.PlannedSecondsAlongPath(simulation.Stops, simulation.StartStopIndex, targetPathIndex)
                : StopPlanItemExtensions.PlannedSecondsAlongPath(simulation.Stops, simulation.StartStopIndex, lastIndex)
                    + StopPlanItemExtensions.PlannedSecondsAlongPath(simulation.Stops, 0, targetPathIndex);

            DateTime agencyNow = _clock.AgencyNow;
            TimeSpan elapsed = _clock.UtcNow - simulation.StartedAtUtc;

            if (elapsed < TimeSpan.Zero)
                elapsed = TimeSpan.Zero;

            DateTime agencyTripStart = agencyNow - elapsed;

            string runLabel = string.Format(
                gmvDomain.Messages.DefaultRunLabelFormat,
                route.Brand(),
                TimeOnly.FromDateTime(agencyTripStart).ToString("HH:mm"));

            double? secondsUntil = _positionCalculator.SecondsUntilArrivalAtStop(
                simulation.Stops,
                simulation.StartStopIndex,
                simulation.SpeedMetersPerSecond,
                simulation.AverageDwellSeconds,
                gmvDomain.AppConstants.DoorClosingSeconds,
                elapsed,
                code);

            double? firstPassSeconds = _positionCalculator.SecondsUntilArrivalAtStop(
                simulation.Stops,
                simulation.StartStopIndex,
                simulation.SpeedMetersPerSecond,
                simulation.AverageDwellSeconds,
                gmvDomain.AppConstants.DoorClosingSeconds,
                TimeSpan.Zero,
                code);

            bool arrivesOnNextTrip = firstPassSeconds is double firstPass
                && elapsed.TotalSeconds > firstPass + simulation.AverageDwellSeconds;

            if (arrivesOnNextTrip)
                plannedSeconds += StopPlanItemExtensions.PlannedSecondsAlongPath(simulation.Stops, 0, lastIndex);

            int arrivalSecondsFromTripStart = plannedSeconds;
            string? status = null;
            TimeOnly plannedClock = TimeOnly.FromDateTime(agencyTripStart.AddSeconds(plannedSeconds));
            TimeOnly? actualClock = null;

            if (secondsUntil is double seconds)
            {
                arrivalSecondsFromTripStart = (int)Math.Round(elapsed.TotalSeconds) + (int)Math.Round(seconds);
                actualClock = TimeOnly.FromDateTime(agencyNow.AddSeconds(seconds));

                int deltaSeconds = arrivalSecondsFromTripStart - plannedSeconds;
                if (Math.Abs(deltaSeconds) <= gmvDomain.AppConstants.ScheduleGraceSeconds)
                    status = gmvDomain.ScheduleStatuses.OnTime;
                else if (deltaSeconds > 0)
                    status = gmvDomain.ScheduleStatuses.RunningLate;
                else
                    status = gmvDomain.ScheduleStatuses.Ahead;
            }

            return ItemFactory.CreateItem<NextArrivalDto>(new
            {
                StopCode = code,
                RunLabel = runLabel,
                PlannedTime = plannedClock,
                ActualTime = actualClock,
                Status = status
            });
        }
    }
}
