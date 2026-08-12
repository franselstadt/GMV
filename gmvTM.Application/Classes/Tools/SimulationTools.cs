using System;
using System.Collections.Generic;
using gmvTM.Domain.Items.View;

namespace gmvTM.Application.Classes.Tools
{
    public static class SimulationTools
    {
        public static int IndexOfStop(IReadOnlyList<PathStopViewItem> stops, string stopCode)
        {
            for (int i = 0; i < stops.Count; i++)
            {
                if (string.Equals(stops[i].StopCode, stopCode, StringComparison.OrdinalIgnoreCase))
                    return i;
            }

            return -1;
        }

        public static int IndexOfStopAfter(IReadOnlyList<PathStopViewItem> stops, int startIndex, string stopCode)
        {
            for (int i = startIndex + 1; i < stops.Count; i++)
            {
                if (string.Equals(stops[i].StopCode, stopCode, StringComparison.OrdinalIgnoreCase))
                    return i;
            }

            return -1;
        }
    }
}
