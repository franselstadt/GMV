using System;
using System.Collections.Generic;
using gmvTM.Domain.Items.Base;

namespace gmvTM.Domain.Items
{
    public sealed class TripItem : BaseItem
    {
        private int routeID;
        private int vehicleID;
        private int startStopID;
        private string status = null!;
        private DateTime startedAtUtc;
        private double averageMph;
        private int averageDwellSeconds;
        private List<StopTripItem> stopTrips = new List<StopTripItem>();

        public override string TableName
        {
            get { return global::gmvTM.Domain.Tables.Trips; }
        }

        [TableDefinition(ForeignKeyOf = typeof(RouteItem), OnDelete = TableDeleteBehavior.Cascade)]
        public int RouteID
        {
            get { return this.routeID; }
            set { this.routeID = value; }
        }

        [TableDefinition(ForeignKeyOf = typeof(VehicleItem), OnDelete = TableDeleteBehavior.Restrict)]
        public int VehicleID
        {
            get { return this.vehicleID; }
            set { this.vehicleID = value; }
        }

        [TableDefinition(ForeignKeyOf = typeof(StopItem), OnDelete = TableDeleteBehavior.Restrict)]
        public int StartStopID
        {
            get { return this.startStopID; }
            set { this.startStopID = value; }
        }

        [TableDefinition(MaxLength = 64, IsRequired = true)]
        public string Status
        {
            get { return this.status; }
            set { this.status = value; }
        }

        public DateTime StartedAtUtc
        {
            get { return this.startedAtUtc; }
            set { this.startedAtUtc = value; }
        }

        public double AverageMph
        {
            get { return this.averageMph; }
            set { this.averageMph = value; }
        }

        public int AverageDwellSeconds
        {
            get { return this.averageDwellSeconds; }
            set { this.averageDwellSeconds = value; }
        }

        [TableDefinition(AutoInclude = true)]
        public List<StopTripItem> StopTrips
        {
            get { return this.stopTrips; }
            set { this.stopTrips = value; }
        }
    }
}
