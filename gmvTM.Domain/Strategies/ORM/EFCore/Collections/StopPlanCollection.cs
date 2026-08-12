using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using gmvTM.Domain.Collections.Interfaces;
using gmvTM.Domain.Strategies.ORM.EFCore.Infrastructure;
using gmvTM.Domain.Items;

namespace gmvTM.Domain.Strategies.ORM.EFCore.Collections
{
    public sealed class StopPlanCollection : BaseCollection<StopPlanItem>, IStopPlanCollection
    {
        public StopPlanCollection(DatabaseContext context) : base(context)
        {
        }

        public IReadOnlyList<StopPlanItem> ReadByRouteID(IStopCollection stops, int routeID)
        {
            ArgumentNullException.ThrowIfNull(stops);

            HashSet<int> stopIds = stops.ReadIDsByRouteID(routeID);
            if (!stopIds.Any())
                return Array.Empty<StopPlanItem>();

            return this.ReadItems()
                .Where(s => stopIds.Contains(s.StopID))
                .OrderBy(s => s.Sequence)
                .ToList();
        }

        public async Task<IReadOnlyList<StopPlanItem>> ReadByRouteIDAsync(IStopCollection stops, int routeID, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(stops);

            HashSet<int> stopIds = await stops.ReadIDsByRouteIDAsync(routeID, cancellationToken).ConfigureAwait(false);
            if (!stopIds.Any())
                return Array.Empty<StopPlanItem>();

            IReadOnlyList<StopPlanItem> all = await this.ReadItemsAsync(cancellationToken).ConfigureAwait(false);

            return all
                .Where(s => stopIds.Contains(s.StopID))
                .OrderBy(s => s.Sequence)
                .ToList();
        }
    }
}
