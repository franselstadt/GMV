using System;
using gmvTM.Domain.Items.Base;

namespace gmvTM.Domain.Items.View
{
    public sealed class ScheduleRunViewItem : ViewItem
    {
        private readonly int runIndex;
        private readonly string runLabel;
        private readonly TimeOnly startTime;

        public ScheduleRunViewItem(int runIndex, string runLabel, TimeOnly startTime)
        {
            this.runIndex = runIndex;
            this.runLabel = runLabel;
            this.startTime = startTime;
        }

        public override string ViewName
        {
            get { return nameof(ScheduleRunViewItem); }
        }

        public int RunIndex
        {
            get { return this.runIndex; }
        }

        public string RunLabel
        {
            get { return this.runLabel; }
        }

        public TimeOnly StartTime
        {
            get { return this.startTime; }
        }
    }
}
