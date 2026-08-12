using gmvTM.Domain.Items.Base;

namespace gmvTM.Domain.Items
{
    public sealed class StopPlanItem : BaseItem
    {
        private int stopID;
        private int sequence;
        private int arrivalSeconds;
        private int runIndex;
        private string runLabel = string.Empty;

        public override string TableName
        {
            get { return global::gmvTM.Domain.Tables.StopPlans; }
        }

        public int StopID
        {
            get { return this.stopID; }
            set { this.stopID = value; }
        }

        public int Sequence
        {
            get { return this.sequence; }
            set { this.sequence = value; }
        }

        public int ArrivalSeconds
        {
            get { return this.arrivalSeconds; }
            set { this.arrivalSeconds = value; }
        }

        [View]
        public int RunIndex
        {
            get { return this.runIndex; }
            set { this.runIndex = value; }
        }

        [View]
        public string RunLabel
        {
            get { return this.runLabel; }
            set { this.runLabel = value ?? string.Empty; }
        }
    }
}
