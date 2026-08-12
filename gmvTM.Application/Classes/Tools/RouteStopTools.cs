using System;
using System.Collections.Generic;
using gmvTM.Application.Classes.Exceptions;
using gmvTM.Domain;
using gmvTM.Domain.Items.View;

namespace gmvTM.Application.Classes.Tools
{
    public static class RouteStopTools
    {
        public static int IndexOfStopOnOrAfter(IReadOnlyList<PathStopViewItem> stops, int startIndex, string stopCode)
        {
            for (int i = startIndex + 1; i < stops.Count; i++)
            {
                if (string.Equals(stops[i].StopCode, stopCode, StringComparison.OrdinalIgnoreCase))
                    return i;
            }

            for (int i = 0; i <= startIndex && i < stops.Count; i++)
            {
                if (string.Equals(stops[i].StopCode, stopCode, StringComparison.OrdinalIgnoreCase))
                    return i;
            }

            return -1;
        }

        public static string RequireStopCode(string stopCode)
        {
            if (string.IsNullOrWhiteSpace(stopCode))
                throw new ValidationException(gmvDomain.Messages.StopCodeRequired);

            return stopCode.Trim();
        }
    }
}
