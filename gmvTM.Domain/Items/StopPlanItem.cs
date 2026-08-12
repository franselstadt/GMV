using gmvTM.Domain.Items.Base;

namespace gmvTM.Domain.Items
{
    public sealed class StopPlanItem : BaseItem
    {
        private int stopID;
        private int sequence;
        private int arrivalSeconds;

        public override string TableName
        {
            get { return gmvDomain.Tables.StopPlans; }
        }

        [TableDefinition(ForeignKeyOf = typeof(StopItem), OnDelete = TableDeleteBehavior.Cascade, IsUnique = true)]
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
    }
}
