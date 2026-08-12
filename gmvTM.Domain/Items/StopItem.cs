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

        public override string TableName
        {
            get { return global::gmvTM.Domain.Tables.Stops; }
        }

        public int RouteID
        {
            get { return this.routeID; }
            set { this.routeID = value; }
        }

        public string StopCode
        {
            get { return this.stopCode; }
            set { this.stopCode = value; }
        }

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
        public int Sequence
        {
            get { return this.sequence; }
            set { this.sequence = value; }
        }
    }
}
