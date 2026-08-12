using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using gmvTM.Domain.Collections.Interfaces;
using gmvTM.Domain.Items;

namespace gmvTM.Domain.Extensions.Collections
{
    public static class StopPlanCollectionExtensions
    {
        public static IReadOnlyList<StopPlanItem> GetByRouteID(this IStopPlanCollection stopPlans, IStopCollection stops, int routeID)
        {
            ArgumentNullException.ThrowIfNull(stopPlans);
            return stopPlans.ReadByRouteID(stops, routeID);
        }

        public static Task<IReadOnlyList<StopPlanItem>> GetByRouteIDAsync(this IStopPlanCollection stopPlans, IStopCollection stops, int routeID, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(stopPlans);
            return stopPlans.ReadByRouteIDAsync(stops, routeID, cancellationToken);
        }
    }
}
