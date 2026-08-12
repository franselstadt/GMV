using gmvTM.Domain.Items;

namespace gmvTM.Domain
{
    public static class VehicleFactory
    {
        public static VehicleItem CreateItem(string fleetCode, string make, string model, string licensePlate, int capacity, int modelYear, bool wheelchairAccessible = false, int id = 0)
        {
            return new VehicleItem
            {
                ID = id,
                FleetCode = fleetCode,
                Make = make,
                Model = model,
                LicensePlate = licensePlate,
                Capacity = capacity,
                ModelYear = modelYear,
                WheelchairAccessible = wheelchairAccessible
            };
        }
    }
}
