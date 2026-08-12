using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using gmvTM.Domain.Extensions.Items;
using gmvTM.Domain.Infrastructure.Persistence;
using gmvTM.Domain.Items;
using gmvTM.Domain.Items.View;
using gmvTM.Domain.Workers.Base;
using gmvTM.Domain.Workers.Interfaces;

namespace gmvTM.Domain.Workers
{
    public sealed class TripPathCalculatorWorker : BaseWorker, ITripPathCalculatorWorker
    {
        private readonly IPolylineDecoderWorker polylineDecoder;
        private readonly IRoutePathBuilderWorker pathBuilder;

        //revisit
        public TripPathCalculatorWorker(DatabaseContext context, IPolylineDecoderWorker polylineDecoder, IRoutePathBuilderWorker pathBuilder): base(context)
        {
            this.polylineDecoder = polylineDecoder
                ?? throw new ArgumentNullException(nameof(polylineDecoder));

            this.pathBuilder = pathBuilder
                ?? throw new ArgumentNullException(nameof(pathBuilder));
        }

        public TripPathViewItem Calculate(int routeID, int runIndex)
        {
            return this.CalculateAsync(routeID, runIndex).GetAwaiter().GetResult();
        }

        public async Task<TripPathViewItem> CalculateAsync(int routeID, int runIndex, CancellationToken cancellationToken = default)
        {
            RouteItem? route = await this.Context.Routes
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.ID == routeID, cancellationToken)
                .ConfigureAwait(false);

            if (route is null)
                throw new InvalidOperationException($"the route with {routeID} was not found");

            List<StopPlanItem> schedule = await (
                    from plan in this.Context.StopPlans.AsNoTracking()
                    join stop in this.Context.Stops.AsNoTracking() on plan.StopID equals stop.ID
                    where stop.RouteID == routeID
                    orderby plan.Sequence
                    select plan)
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);

            if (schedule.Count < 2)
                throw new InvalidOperationException(
                    $"schedule on route with id{routeID} needs at least two stops");

            ScheduleRunViewItem? run = ScheduleRunViewItemExtensions.GetByIndex(route, runIndex);
            if (run is not null)
                run.ApplyTo(schedule);

            HashSet<int> stopIDs = schedule.Select(s => s.StopID).ToHashSet();
            Dictionary<int, StopItem> stopsByID = await this.Context.Stops
                .AsNoTracking()
                .Where(s => stopIDs.Contains(s.ID))
                .ToDictionaryAsync(s => s.ID, cancellationToken)
                .ConfigureAwait(false);

            RoutePathViewItem path = this.pathBuilder.Build(
                this.polylineDecoder.Decode(route.EncodedPolyline));

            List<PathStopViewItem> ordered = schedule
                .Select(s =>
                {
                    StopItem stop = stopsByID[s.StopID];
                    CoordinatesViewItem coords = new CoordinatesViewItem(stop.Latitude, stop.Longitude);
                    return new PathStopViewItem(
                        stop.ID,
                        stop.StopCode,
                        stop.Name,
                        path.NearestDistanceMeters(coords),
                        s.ArrivalSeconds);
                })
                .ToList();

            for (int i = 1; i < ordered.Count; i++)
            {
                if (ordered[i].DistanceAlongPathMeters < ordered[i - 1].DistanceAlongPathMeters)
                    ordered[i] = ordered[i].WithDistance(ordered[i - 1].DistanceAlongPathMeters);
            }

            PathStopViewItem origin = ordered[0];
            double closeDistance = Math.Max(path.TotalMeters, ordered[ordered.Count - 1].DistanceAlongPathMeters);
            
            ordered.Add(new PathStopViewItem(
                origin.StopID,
                origin.StopCode,
                origin.Name,
                closeDistance,
                origin.PlannedArrivalSeconds));

            return new TripPathViewItem(path, ordered);
        }
    }
}
