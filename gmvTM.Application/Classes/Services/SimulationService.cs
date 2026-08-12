using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using gmvTM.Application.Classes.Exceptions;
using gmvTM.Application.Classes.Simulation;
using gmvTM.Application.Classes.Tools;
using gmvTM.Application.Extensions;
using gmvTM.Application.Interfaces;
using gmvTM.Domain;
using gmvTM.Domain.Classes.Tools;
using gmvTM.Domain.Collections.Interfaces;
using gmvTM.Domain.Extensions.Collections;
using gmvTM.Domain.Extensions.Items;
using gmvTM.Domain.Infrastructure.Interfaces;
using gmvTM.Domain.Items;
using gmvTM.Domain.Items.View;
using gmvTM.Domain.Workers.Interfaces;

namespace gmvTM.Application.Classes.Services
{
    // Claude, Google, YouTube, and many other resources assisted in my understanding of how to
    // architect and write this code, especially from a math perspective. I remembered a lot from a
    // previous, similar project, but in this example we simulate, which was a neat new challenge
    // for me. I learned a lot about how to simulate a vehicle moving along a route (for display on
    // Leaflet) and how to calculate its position and status based on time and speed. It is by no
    // means perfect, as there are many more variables. I also learned how to handle multiple
    // simulations and how to broadcast the vehicle position to clients. I know how to do this
    // using Kafka/RabbitMQ or even good old Socket.IO, but I have not used SignalR in a while,
    // other than Blazor diff streaming.
    public sealed class SimulationService : ISimulationService
    {
        private readonly ISimpleUnitOfWork _unitOfWork;
        private readonly ITripPathCalculatorWorker _tripPathCalculator;
        private readonly ITripPositionCalculatorWorker _positionCalculator;
        private readonly ISimulationStore _store;
        private readonly IVehiclePositionBroadcaster _broadcaster;
        private readonly IDateTimeProviderWorker _clock;

        public SimulationService(ISimpleUnitOfWork unitOfWork, ITripPathCalculatorWorker tripPathCalculator, ITripPositionCalculatorWorker positionCalculator, ISimulationStore store, IVehiclePositionBroadcaster broadcaster, IDateTimeProviderWorker clock)
        {
            _unitOfWork = unitOfWork;
            _tripPathCalculator = tripPathCalculator;
            _positionCalculator = positionCalculator;
            _store = store;
            _broadcaster = broadcaster;
            _clock = clock;
        }

        private IRouteCollection Routes => _unitOfWork.Routes;

        private IVehicleCollection Vehicles => _unitOfWork.Vehicles;

        private IStopCollection Stops => _unitOfWork.Stops;

        private IStopPlanCollection StopPlans => _unitOfWork.StopPlans;

        private ITripCollection Trips => _unitOfWork.Trips;

        private IStopTripCollection StopTrips => _unitOfWork.StopTrips;


        // Claude assisted greatly in my understanding of how to architect and write this code,
        // especially from a math perspective. I remembered a lot from a previous, similar project,
        // but in this example we simulate, which was a neat new challenge for me. I learned a lot
        // about how to simulate a vehicle moving along a route and how to calculate its position
        // and status based on time and speed. I also learned how to handle multiple simulations
        // and how to broadcast the vehicle position to clients. This was a fun and educational
        // project for me.
        public async Task<SimulationRunDto> StartAsync(string routeCode, StartSimulationRequest request, CancellationToken ct)
        {
            RouteItem route = await RouteCodeLookup.RequireAsync(Routes, routeCode, ct);
            string normalizedRouteCode = route.ShortName.Trim().ToUpperInvariant();

            double mph = request.AverageMph ?? gmvDomain.AppConstants.DefaultAverageMph;
            if (mph < gmvDomain.AppConstants.MinAverageMph || mph > gmvDomain.AppConstants.MaxAverageMph)
            {
                throw new ValidationException(
                    string.Format(gmvDomain.Messages.MphOutOfRange, gmvDomain.AppConstants.MinAverageMph, gmvDomain.AppConstants.MaxAverageMph));
            }

            int dwell = request.AverageDwellSeconds ?? gmvDomain.AppConstants.DefaultAverageDwellSeconds;
            if (dwell < gmvDomain.AppConstants.MinAverageDwellSeconds || dwell > gmvDomain.AppConstants.MaxAverageDwellSeconds)
            {
                throw new ValidationException(
                    string.Format(gmvDomain.Messages.AverageDwellOutOfRange, gmvDomain.AppConstants.MinAverageDwellSeconds, gmvDomain.AppConstants.MaxAverageDwellSeconds));
            }

            VehicleItem vehicle = await ResolveVehicleAsync(request.VehicleID, ct);

            IReadOnlyList<StopPlanItem> schedule = await StopPlans.GetByRouteIDAsync(Stops, route.ID, ct);
            if (schedule.Count < 2)
                throw new ValidationException(string.Format(gmvDomain.Messages.NoScheduleForRoute, route.ShortName));

            TripPathViewItem tripPath = await _tripPathCalculator.CalculateAsync(route.ID, ct);
            IReadOnlyList<PathStopViewItem> ordered = tripPath.Stops;
            if (ordered.Count < 2)
                throw new ValidationException(string.Format(gmvDomain.Messages.NoScheduleForRoute, route.ShortName));

            int startIndex = 0;
            if (!string.IsNullOrWhiteSpace(request.StopCode))
            {
                string stopCode = request.StopCode.Trim();
                startIndex = SimulationTools.IndexOfStop(ordered, stopCode);
                if (startIndex < 0 || startIndex >= ordered.Count - 1)
                {
                    throw new ValidationException(
                        string.Format(gmvDomain.Messages.StopNotOnSchedule, stopCode, route.ShortName));
                }
            }

            DateTime now = _clock.UtcNow;

            foreach (ActiveSimulation prior in _store.ListActive().ToList())
            {
                prior.Status = gmvDomain.Messages.SimulationStatusStopped;
                TripItem? priorTrip = await Trips.ReadAsync(prior.TripID, ct);
                if (priorTrip is not null)
                {
                    priorTrip.Status = gmvDomain.Messages.SimulationStatusStopped;
                    await Trips.UpdateAsync(priorTrip, ct);
                }

                _store.TryRemove(prior.ID, out _);
            }

            TripItem trip = await StageNewTripAsync(
                route,
                vehicle.ID,
                ordered,
                startIndex,
                mph,
                dwell,
                now,
                ct);

            await _unitOfWork.SaveChangesAsync(ct);

            ActiveSimulation simulation = new ActiveSimulation(
                id: trip.ID,
                routeCode: normalizedRouteCode,
                vehicleID: vehicle.ID,
                vehicleNumber: vehicle.FleetCode,
                tripID: trip.ID,
                status: gmvDomain.Messages.SimulationStatusRunning,
                startStopCode: ordered[startIndex].StopCode,
                averageMph: mph,
                averageDwellSeconds: dwell,
                startedAtUtc: now,
                startStopIndex: startIndex,
                speedMetersPerSecond: TravelTools.MetersPerSecondFromMph(mph),
                path: tripPath.Path,
                stops: ordered,
                stopTrips: trip.StopTrips.ToList());

            _store.Add(simulation);
            return simulation.ToDto();
        }

        public async Task StopAsync(int simulationRunID, CancellationToken ct)
        {
            if (!_store.TryGet(simulationRunID, out ActiveSimulation? simulation) || simulation is null)
                throw new NotFoundException(string.Format(gmvDomain.Messages.SimulationNotFound, simulationRunID));

            simulation.Status = gmvDomain.Messages.SimulationStatusStopped;
            TripItem? trip = await Trips.ReadAsync(simulation.TripID, ct);
            if (trip is not null)
            {
                trip.Status = gmvDomain.Messages.SimulationStatusStopped;
                await Trips.UpdateAsync(trip, ct);
                await _unitOfWork.SaveChangesAsync(ct);
            }

            _store.TryRemove(simulationRunID, out _);
        }

        public Task<IReadOnlyList<SimulationRunDto>> GetActiveAsync(CancellationToken ct)
        {
            IReadOnlyList<SimulationRunDto> list = _store.ListActive()
                .Select(simulation => simulation.ToDto())
                .ToList();

            return Task.FromResult(list);
        }

        public async Task TickAsync(CancellationToken ct)
        {

            //simulate vehicle movement event on ticket stream back to ws to map on leaflet map
            DateTime now = _clock.UtcNow;

            foreach (ActiveSimulation simulation in _store.ListActive().ToList())
            {
                TimeSpan elapsed = now - simulation.StartedAtUtc;
                VehicleMotionViewItem motion = _positionCalculator.Calculate(
                    simulation.Path,
                    simulation.Stops,
                    simulation.StartStopIndex,
                    simulation.SpeedMetersPerSecond,
                    simulation.AverageDwellSeconds,
                    gmvDomain.AppConstants.AnnounceLeadSeconds,
                    gmvDomain.AppConstants.DoorClosingSeconds,
                    elapsed);

                if (string.Equals(motion.Phase, gmvDomain.VehiclePhases.Completed, StringComparison.Ordinal))
                {
                    await BeginNextTripAfterRoundAsync(simulation, now, ct);

                    motion = _positionCalculator.Calculate(
                        simulation.Path,
                        simulation.Stops,
                        simulation.StartStopIndex,
                        simulation.SpeedMetersPerSecond,
                        simulation.AverageDwellSeconds,
                        gmvDomain.AppConstants.AnnounceLeadSeconds,
                        gmvDomain.AppConstants.DoorClosingSeconds,
                        TimeSpan.Zero);
                }

                bool dirty = await UpdateStopTripTelemetryAsync(simulation, motion, now, ct);
                if (dirty)
                    await _unitOfWork.SaveChangesAsync(ct);

                VehiclePositionDto dto = ItemFactory.CreateItem<VehiclePositionDto>(new
                {
                    SimulationRunID = simulation.ID,
                    TripID = simulation.TripID,
                    RouteCode = simulation.RouteCode,
                    VehicleID = simulation.VehicleID,
                    VehicleNumber = simulation.VehicleNumber,
                    Latitude = motion.Position.Latitude,
                    Longitude = motion.Position.Longitude,
                    Phase = motion.Phase,
                    StopCode = motion.StopCode,
                    StopName = motion.StopName,
                    SecondsToStop = motion.SecondsToStop,
                    BehindSchedule = !string.IsNullOrWhiteSpace(simulation.ScheduleAlert),
                    ScheduleAlert = simulation.ScheduleAlert,
                    AsOfUtc = now
                });

                await _broadcaster.BroadcastAsync(dto, ct);
            }
        }

        private async Task<bool> UpdateStopTripTelemetryAsync(ActiveSimulation simulation, VehicleMotionViewItem motion, DateTime utcNow, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(motion.StopCode))
                return false;

            PathStopViewItem? pathStop = simulation.Stops.FirstOrDefault(s =>
                string.Equals(s.StopCode, motion.StopCode, StringComparison.OrdinalIgnoreCase));

            if (pathStop is null)
                return false;

            StopTripItem? tripStop = simulation.StopTrips.FirstOrDefault(t => t.StopID == pathStop.StopID);
            if (tripStop is null)
                return false;

            bool dirty = false;

            if (motion.SecondsToStop is double seconds)
            {
                int pathIndex = SimulationTools.IndexOfStopAfter(simulation.Stops, simulation.StartStopIndex, motion.StopCode!);
                if (pathIndex >= 0)
                {
                    int actualSeconds = (int)Math.Round((utcNow - simulation.StartedAtUtc).TotalSeconds)
                        + (int)Math.Round(seconds);

                    if (tripStop.ActualArrivalSeconds != actualSeconds)
                    {
                        tripStop.ActualArrivalSeconds = actualSeconds;
                        dirty = true;
                    }

                    int plannedSeconds = StopPlanItemExtensions.PlannedSecondsAlongPath(
                        simulation.Stops,
                        simulation.StartStopIndex,
                        pathIndex);

                    bool behind = actualSeconds > plannedSeconds + gmvDomain.AppConstants.ScheduleGraceSeconds;

                    if (behind)
                    {
                        string alert = string.Format(
                            gmvDomain.Messages.BehindScheduleAlert,
                            pathStop.Name,
                            actualSeconds,
                            plannedSeconds);


                        simulation.ScheduleAlert = alert;

                        if (!tripStop.BehindSchedule)
                        {
                            tripStop.BehindSchedule = true;
                            dirty = true;
                        }
                    }
                }
            }

            if (string.Equals(motion.Phase, gmvDomain.VehiclePhases.DoorsOpen, StringComparison.Ordinal) && tripStop.ActualArrivalUtc is null)
            {
                int pathIndex = SimulationTools.IndexOfStopAfter(simulation.Stops, simulation.StartStopIndex, motion.StopCode!);
                int actualSeconds = (int)Math.Round((utcNow - simulation.StartedAtUtc).TotalSeconds);

                tripStop.ActualArrivalUtc = utcNow;
                tripStop.ActualArrivalSeconds = actualSeconds;
                dirty = true;


                int plannedSeconds = pathIndex >= 0
                    ? StopPlanItemExtensions.PlannedSecondsAlongPath(
                        simulation.Stops,
                        simulation.StartStopIndex,
                        pathIndex)
                    : 0;



                if (actualSeconds > plannedSeconds + gmvDomain.AppConstants.ScheduleGraceSeconds)
                {
                    tripStop.BehindSchedule = true;
                    simulation.ScheduleAlert = string.Format(
                        gmvDomain.Messages.BehindScheduleAlert,
                        pathStop.Name,
                        actualSeconds,
                        plannedSeconds);
                    dirty = true;
                }
            }

            if (string.Equals(motion.Phase, gmvDomain.VehiclePhases.DoorsClosing, StringComparison.Ordinal) && tripStop.ActualDwellSeconds is null  && tripStop.ActualArrivalUtc is DateTime arrivedAt)
            {
                int elapsedOpen = (int)Math.Round((utcNow - arrivedAt).TotalSeconds);
                tripStop.ActualDwellSeconds = elapsedOpen + gmvDomain.AppConstants.DoorClosingSeconds;
                dirty = true;
            }

            if (dirty)
                await StopTrips.UpdateAsync(tripStop, ct);

            return dirty;
        }

        private async Task BeginNextTripAfterRoundAsync(ActiveSimulation simulation, DateTime nowUtc, CancellationToken ct)
        {
            TripItem? completed = await Trips.ReadAsync(simulation.TripID, ct);
            if (completed is not null)
            {
                completed.Status = gmvDomain.Messages.SimulationStatusCompleted;
                await Trips.UpdateAsync(completed, ct);
            }

            int routeID = completed?.RouteID
                ?? throw new InvalidOperationException(
                    string.Format(gmvDomain.Messages.SimulationNotFound, simulation.TripID));

            RouteItem? route = await Routes.ReadAsync(routeID, ct);
            if (route is null)
                throw new NotFoundException(string.Format(gmvDomain.Messages.RouteNotFound, simulation.RouteCode));

            TripItem nextTrip = await StageNewTripAsync(
                route,
                simulation.VehicleID,
                simulation.Stops,
                0,
                simulation.AverageMph,
                simulation.AverageDwellSeconds,
                nowUtc,
                ct);

            await _unitOfWork.SaveChangesAsync(ct);

            simulation.TripID = nextTrip.ID;
            simulation.StopTrips = nextTrip.StopTrips.ToList();
            simulation.StartedAtUtc = nowUtc;
            simulation.StartStopIndex = 0;
            simulation.StartStopCode = simulation.Stops[0].StopCode;
            simulation.ScheduleAlert = null;
            simulation.Status = gmvDomain.Messages.SimulationStatusRunning;
        }

        private async Task<TripItem> StageNewTripAsync(RouteItem route, int vehicleID, IReadOnlyList<PathStopViewItem> ordered, int startIndex, double mph, int dwell, DateTime startedAtUtc, CancellationToken ct)
        {
            TripItem trip = TripFactory.CreateItem(route.ID, vehicleID, ordered, startIndex, gmvDomain.Messages.SimulationStatusRunning, startedAtUtc, mph, dwell);

            await Trips.CreateAsync(trip, ct);
            return trip;
        }

        private async Task<VehicleItem> ResolveVehicleAsync(int? vehicleID, CancellationToken ct)
        {
            if (vehicleID is int requestedID)
            {
                VehicleItem? requested = await Vehicles.ReadAsync(requestedID, ct);
                if (requested is null)
                    throw new ValidationException(string.Format(gmvDomain.Messages.VehicleNotFound, requestedID));

                return requested;
            }

            VehicleItem? vehicle = await Vehicles.GetFirstAsync(ct);
            if (vehicle is null)
                throw new ValidationException(gmvDomain.Messages.NoVehicleAvailable);

            return vehicle;
        }
    }
}
