using System;
using System.Collections.Generic;
using System.Linq;
using gmvTM.Domain.Items;
using gmvTM.Domain.Items.View;

namespace gmvTM.Domain.Extensions.Items
{
    public static class StopPlanItemExtensions
    {
        public static TimeOnly ParseRunStartFromLabel(string runLabel)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(runLabel);
            string[] parts = runLabel.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            if (!parts.Any() || !TimeOnly.TryParse(parts[^1], out TimeOnly start))
                return TimeOnly.MinValue;

            return start;
        }

        public static int PlannedSecondsBetweenSequences(IEnumerable<StopPlanItem> runStops, int fromSequence, int toSequence)
        {
            ArgumentNullException.ThrowIfNull(runStops);
            if (fromSequence == toSequence)
                return 0;

            List<StopPlanItem> ordered = runStops.OrderBy(s => s.Sequence).ToList();
            if (!ordered.Any())
                return 0;

            int total = 0;
            bool started = false;

            foreach (StopPlanItem stop in ordered)
            {
                if (!started)
                {
                    if (stop.Sequence == fromSequence)
                        started = true;

                    continue;
                }

                total += stop.ArrivalSeconds;
                if (stop.Sequence == toSequence)
                    return total;
            }

            foreach (StopPlanItem stop in ordered)
            {
                total += stop.ArrivalSeconds;
                if (stop.Sequence == toSequence)
                    return total;
            }

            throw new InvalidOperationException(
                $"Could not walk schedule from sequence {fromSequence} to {toSequence}.");
        }

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
                throw new ArgumentException("Path indices must be in trip order (toIndex >= fromIndex).");

            int total = 0;
            foreach (PathStopViewItem stop in stopsInOrder.Skip(fromIndex + 1).Take(toIndex - fromIndex))
            {
                total += stop.PlannedArrivalSeconds;
            }

            return total;
        }

        public static TimeOnly ToClockTime(string runLabel, int cumulativeSecondsFromStart)
        {
            TimeOnly runStart = ParseRunStartFromLabel(runLabel);
            return runStart.Add(TimeSpan.FromSeconds(cumulativeSecondsFromStart));
        }
    }
}
