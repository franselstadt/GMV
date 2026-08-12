using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using gmvTM.Domain.Collections.Interfaces;
using gmvTM.Domain.Extensions.Items;
using gmvTM.Domain.Items;
using gmvTM.Domain.Items.View;

namespace gmvTM.Domain.Extensions.Collections
{
    public static class StopPlanCollectionExtensions
    {
        public static IReadOnlyList<StopPlanItem> GetByRouteID(this IStopPlanCollection stopPlans, IStopCollection stops, int routeID)
        {
            ArgumentNullException.ThrowIfNull(stopPlans);
            ArgumentNullException.ThrowIfNull(stops);

            HashSet<int> stopIds = stops.GetIDsByRouteID(routeID);
            if (!stopIds.Any())
                return Array.Empty<StopPlanItem>();

            return stopPlans.ReadItems()
                .Where(s => stopIds.Contains(s.StopID))
                .OrderBy(s => s.Sequence)
                .ToList();
        }

        public static async Task<IReadOnlyList<StopPlanItem>> GetByRouteIDAsync(this IStopPlanCollection stopPlans, IStopCollection stops, int routeID, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(stopPlans);
            ArgumentNullException.ThrowIfNull(stops);
            HashSet<int> stopIds = await stops.GetIDsByRouteIDAsync(routeID, cancellationToken)
                .ConfigureAwait(false);

            if (!stopIds.Any())
                return Array.Empty<StopPlanItem>();

            IReadOnlyList<StopPlanItem> all = await stopPlans.ReadItemsAsync(cancellationToken)
                .ConfigureAwait(false);

            return all
                .Where(s => stopIds.Contains(s.StopID))
                .OrderBy(s => s.Sequence)
                .ToList();
        }

        public static IReadOnlyList<StopPlanItem> GetRun(this IStopPlanCollection stopPlans, IStopCollection stops, RouteItem route, int runIndex)
        {
            ArgumentNullException.ThrowIfNull(stopPlans);
            ArgumentNullException.ThrowIfNull(route);

            IReadOnlyList<StopPlanItem> plans = stopPlans.GetByRouteID(stops, route.ID);
            ScheduleRunViewItem? run = ScheduleRunViewItemExtensions.GetByIndex(route, runIndex);

            if (run is not null)
                run.ApplyTo(plans);

            return plans;
        }

        public static async Task<IReadOnlyList<StopPlanItem>> GetRunAsync(this IStopPlanCollection stopPlans, IStopCollection stops, RouteItem route, int runIndex, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(stopPlans);
            ArgumentNullException.ThrowIfNull(route);

            IReadOnlyList<StopPlanItem> plans = await stopPlans
                .GetByRouteIDAsync(stops, route.ID, cancellationToken)
                .ConfigureAwait(false);

            ScheduleRunViewItem? run = ScheduleRunViewItemExtensions.GetByIndex(route, runIndex);

            if (run is not null)
                run.ApplyTo(plans);

            return plans;
        }
    }
}
