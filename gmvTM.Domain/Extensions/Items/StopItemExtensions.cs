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
    }
}
