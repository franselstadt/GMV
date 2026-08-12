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
    public sealed class NextArrivalsCalculatorWorker : BaseWorker, INextArrivalsCalculatorWorker
    {
        public NextArrivalsCalculatorWorker(DatabaseContext context): base(context) {}

        public IReadOnlyList<NextArrivalDto> Calculate(int routeID, int stopID, string stopCode, int count, DateTime agencyNow)
        {
            return this.CalculateAsync(routeID, stopID, stopCode, count, agencyNow)
                .GetAwaiter()
                .GetResult();
        }

        public async Task<IReadOnlyList<NextArrivalDto>> CalculateAsync(int routeID, int stopID, string stopCode, int count, DateTime agencyNow, CancellationToken cancellationToken = default)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(stopCode);

            if (count < 1)
                throw new ArgumentOutOfRangeException(nameof(count));

            string code = stopCode.Trim();

            RouteItem? route = await this.Context.Routes
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.ID == routeID, cancellationToken)
                .ConfigureAwait(false);

            if (route is null)
                return Array.Empty<NextArrivalDto>();

            List<StopPlanItem> pattern = await (
                    from plan in this.Context.StopPlans.AsNoTracking()
                    join stop in this.Context.Stops.AsNoTracking() on plan.StopID equals stop.ID
                    where stop.RouteID == routeID
                    orderby plan.Sequence
                    select plan)
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);

            StopPlanItem? atStop = pattern.FirstOrDefault(p => p.StopID == stopID);

            if (atStop is null || !pattern.Any())
                return Array.Empty<NextArrivalDto>();

            int fromSequence = pattern[0].Sequence;


            int cumulative = StopPlanItemExtensions.PlannedSecondsBetweenSequences(
                pattern,
                fromSequence,
                atStop.Sequence);

            IReadOnlyList<ScheduleRunViewItem> runs = ScheduleRunViewItemExtensions.ForRoute(route);




            return runs
                .Take(count)
                .Select(run => new NextArrivalDto
                {
                    StopCode = code,
                    RunLabel = run.RunLabel,
                    PlannedTime = StopPlanItemExtensions.ToClockTime(run.RunLabel, cumulative),
                    ActualTime = null,
                    Status = null
                })
                .ToList();
        }
    }
}
