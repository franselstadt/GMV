using System;
using gmvTM.Domain.Items;

namespace gmvTM.Domain.Extensions.Items
{
    public static class VehicleItemExtensions
    {
        public static VehicleDto ToDto(this VehicleItem vehicle)
        {
            ArgumentNullException.ThrowIfNull(vehicle);

            return new VehicleDto
            {
                ID = vehicle.ID,
                FleetCode = vehicle.FleetCode,
                Make = vehicle.Make,
                Model = vehicle.Model,
                LicensePlate = vehicle.LicensePlate,
                Capacity = vehicle.Capacity,
                ModelYear = vehicle.ModelYear,
                WheelchairAccessible = vehicle.WheelchairAccessible
            };
        }
    }
}
