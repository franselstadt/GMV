using System;
using gmvTM.Domain.Items;

namespace gmvTM.Domain.Extensions.Items
{
    public static class StopItemExtensions
    {
        public static bool MatchesCode(this StopItem stop, string stopCode)
        {
            ArgumentNullException.ThrowIfNull(stop);

            if (string.IsNullOrWhiteSpace(stopCode))
                return false;

            return string.Equals(stop.StopCode.Trim(), stopCode.Trim(), StringComparison.OrdinalIgnoreCase);
        }

        public static StopDto ToDto(this StopItem stop)
        {
            ArgumentNullException.ThrowIfNull(stop);

            return new StopDto
            {
                ID = stop.ID,
                StopCode = stop.StopCode,
                Name = stop.Name,
                Latitude = stop.Latitude,
                Longitude = stop.Longitude,
                Sequence = stop.Sequence
            };
        }
    }
}
