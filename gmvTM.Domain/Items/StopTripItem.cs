using System;
using gmvTM.Domain.Items.Base;

namespace gmvTM.Domain.Items
{
    public sealed class StopTripItem : BaseItem
    {
        private int tripID;
        private int stopID;
        private string stopCode = null!;
        private string name = null!;
        private int sequence;
        private int arrivalSeconds;
        private double speedMph;
        private int plannedDwellSeconds;
        private int? actualDwellSeconds;
        private int? actualArrivalSeconds;
        private DateTime? actualArrivalUtc;
        private bool behindSchedule;

        public override string TableName
        {
            get { return global::gmvTM.Domain.Tables.StopTrips; }
        }

        public int TripID
        {
            get { return this.tripID; }
            set { this.tripID = value; }
        }

        public int StopID
        {
            get { return this.stopID; }
            set { this.stopID = value; }
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

        public double SpeedMph
        {
            get { return this.speedMph; }
            set { this.speedMph = value; }
        }

        public int PlannedDwellSeconds
        {
            get { return this.plannedDwellSeconds; }
            set { this.plannedDwellSeconds = value; }
        }

        public int? ActualDwellSeconds
        {
            get { return this.actualDwellSeconds; }
            set { this.actualDwellSeconds = value; }
        }

        public int? ActualArrivalSeconds
        {
            get { return this.actualArrivalSeconds; }
            set { this.actualArrivalSeconds = value; }
        }

        public DateTime? ActualArrivalUtc
        {
            get { return this.actualArrivalUtc; }
            set { this.actualArrivalUtc = value; }
        }

        public bool BehindSchedule
        {
            get { return this.behindSchedule; }
            set { this.behindSchedule = value; }
        }
    }
}
