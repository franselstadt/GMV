using System;
using System.Collections.Generic;
using System.Linq;
using gmvTM.Domain.Items;
using gmvTM.Domain.Items.View;

namespace gmvTM.Domain.Extensions.Items
{
    public static class StopPlanItemExtensions
    {
        public static int PlannedSecondsAlongPath(IReadOnlyList<PathStopViewItem> stopsInOrder, int fromIndex, int toIndex)
        {
            ArgumentNullException.ThrowIfNull(stopsInOrder);
            if (fromIndex < 0 || fromIndex >= stopsInOrder.Count)
                throw new ArgumentOutOfRangeException(nameof(fromIndex));

            if (toIndex < 0 || toIndex >= stopsInOrder.Count)
                throw new ArgumentOutOfRangeException(nameof(toIndex));

            if (fromIndex == toIndex)
                return 0;

            if (toIndex < fromIndex)
                throw new ArgumentException(gmvDomain.Messages.PathIndicesOutOfOrder);

            int total = 0;
            foreach (PathStopViewItem stop in stopsInOrder.Skip(fromIndex + 1).Take(toIndex - fromIndex))
            {
                total += stop.PlannedArrivalSeconds;
            }

            return total;
        }
    }
}
