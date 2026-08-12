using System.Collections.Generic;
using gmvTM.Domain.Items.Base;

namespace gmvTM.Domain.Items
{
    public sealed class VehicleItem : BaseItem
    {
        private string fleetCode = null!;
        private string make = null!;
        private string model = null!;
        private string licensePlate = null!;
        private int capacity;
        private int modelYear;
        private bool wheelchairAccessible;
        private List<TripItem> trips = new List<TripItem>();

        public override string TableName
        {
            get { return global::gmvTM.Domain.Tables.Vehicles; }
        }

        public string FleetCode
        {
            get { return this.fleetCode; }
            set { this.fleetCode = value; }
        }

        public string Make
        {
            get { return this.make; }
            set { this.make = value; }
        }

        public string Model
        {
            get { return this.model; }
            set { this.model = value; }
        }

        public string LicensePlate
        {
            get { return this.licensePlate; }
            set { this.licensePlate = value; }
        }

        public int Capacity
        {
            get { return this.capacity; }
            set { this.capacity = value; }
        }

        public int ModelYear
        {
            get { return this.modelYear; }
            set { this.modelYear = value; }
        }

        public bool WheelchairAccessible
        {
            get { return this.wheelchairAccessible; }
            set { this.wheelchairAccessible = value; }
        }

        public List<TripItem> Trips
        {
            get { return this.trips; }
            set { this.trips = value; }
        }
    }
}
