using System;
using System.Collections.Generic;
using System.Linq;
using gmvTM.Domain.Items;
using gmvTM.Domain.Items.View;

namespace gmvTM.Domain.Extensions.Items
{
    public static class ScheduleRunViewItemExtensions
    {
        private static readonly string[] RunStartTimes =
        {
            "07:00",
            "12:00",
            "16:00",
            "19:50"
        };

        public static IReadOnlyList<ScheduleRunViewItem> ForRoute(RouteItem route)
        {
            ArgumentNullException.ThrowIfNull(route);
            string brand = route.Brand();

            List<ScheduleRunViewItem> runs = new List<ScheduleRunViewItem>(RunStartTimes.Length);
            int runIndex = 0;
            foreach (string startTime in RunStartTimes)
            {
                string label = string.Format(Messages.DefaultRunLabelFormat, brand, startTime);
                runs.Add(new ScheduleRunViewItem(runIndex, label, StopPlanItemExtensions.ParseRunStartFromLabel(label)));
                runIndex++;
            }

            return runs;
        }

        public static ScheduleRunViewItem? GetByIndex(RouteItem route, int runIndex)
        {
            return ForRoute(route).FirstOrDefault(r => r.RunIndex == runIndex);
        }

        public static ScheduleRunViewItem PickActive(RouteItem route, DateTime agencyNow)
        {
            IReadOnlyList<ScheduleRunViewItem> runs = ForRoute(route);
            if (!runs.Any())
                throw new InvalidOperationException(string.Format(Messages.NoScheduleForRoute, route.ShortName));

            TimeOnly now = TimeOnly.FromDateTime(agencyNow);
            ScheduleRunViewItem? active = null;

            foreach (ScheduleRunViewItem run in runs.OrderBy(r => r.StartTime))
            {
                if (run.StartTime <= now)
                    active = run;
            }

            return active ?? runs.OrderByDescending(r => r.StartTime).First();
        }

        public static ScheduleRunViewItem PickClosest(RouteItem route, DateTime agencyNow)
        {
            return PickActive(route, agencyNow);
        }

        public static void ApplyTo(this ScheduleRunViewItem run, IEnumerable<StopPlanItem> plans)
        {
            ArgumentNullException.ThrowIfNull(run);
            ArgumentNullException.ThrowIfNull(plans);

            foreach (StopPlanItem plan in plans)
            {
                plan.RunIndex = run.RunIndex;
                plan.RunLabel = run.RunLabel;
            }
        }
    }
}
