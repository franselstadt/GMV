using System.Collections.Generic;
using System.Linq;
using gmvTM.Domain.Items;
using gmvTM.Domain.Items.View;

namespace gmvTM.Domain
{
    public static class StopTripFactory
    {
        public static StopTripItem CreateItem(int stopID, string stopCode, string name, int sequence, int arrivalSeconds, double speedMph, int plannedDwellSeconds)
        {
            return new StopTripItem
            {
                StopID = stopID,
                StopCode = stopCode,
                Name = name,
                Sequence = sequence,
                ArrivalSeconds = arrivalSeconds,
                SpeedMph = speedMph,
                PlannedDwellSeconds = plannedDwellSeconds
            };
        }

        public static List<StopTripItem> CreateItems(IReadOnlyList<PathStopViewItem> stopsInOrder, int startIndex, double speedMph, int plannedDwellSeconds)
        {
            List<StopTripItem> items = new List<StopTripItem>();

            int sequence = 0;
            foreach (PathStopViewItem stop in stopsInOrder.Skip(startIndex))
            {
                items.Add(CreateItem(stop.StopID, stop.StopCode, stop.Name, sequence, stop.PlannedArrivalSeconds, speedMph, plannedDwellSeconds));
                sequence++;
            }

            return items;
        }
    }
}
