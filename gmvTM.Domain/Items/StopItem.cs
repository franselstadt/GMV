using gmvTM.Domain.Items.Base;

namespace gmvTM.Domain.Items
{
    public sealed class StopItem : BaseItem
    {
        private int routeID;
        private string stopCode = null!;
        private string name = null!;
        private double latitude;
        private double longitude;
        private int sequence;
        private string? specialAlert;

        public override string TableName
        {
            get { return gmvDomain.Tables.Stops; }
        }

        [TableDefinition(ForeignKeyOf = typeof(RouteItem), OnDelete = TableDeleteBehavior.Cascade, UniqueGroup = "RouteStopCode")]
        [TableDefinition(UniqueGroup = "RouteSequence")]
        public int RouteID
        {
            get { return this.routeID; }
            set { this.routeID = value; }
        }

        [TableDefinition(MaxLength = 64, IsRequired = true, UniqueGroup = "RouteStopCode")]
        public string StopCode
        {
            get { return this.stopCode; }
            set { this.stopCode = value; }
        }

        [TableDefinition(MaxLength = 256, IsRequired = true)]
        public string Name
        {
            get { return this.name; }
            set { this.name = value; }
        }

        public double Latitude
        {
            get { return this.latitude; }
            set { this.latitude = value; }
        }

        public double Longitude
        {
            get { return this.longitude; }
            set { this.longitude = value; }
        }
        [TableDefinition(UniqueGroup = "RouteSequence")]
        public int Sequence
        {
            get { return this.sequence; }
            set { this.sequence = value; }
        }

        [TableDefinition(MaxLength = 256)]
        public string? SpecialAlert
        {
            get { return this.specialAlert; }
            set { this.specialAlert = value; }
        }
    }
}
