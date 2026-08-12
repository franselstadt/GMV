using System;
using gmvTM.Domain.Items;

namespace gmvTM.Domain.Extensions.Items
{
    public static class RouteItemExtensions
    {
        public static bool MatchesCode(this RouteItem route, string routeCode)
        {
            ArgumentNullException.ThrowIfNull(route);

            if (string.IsNullOrWhiteSpace(routeCode))
                return false;

            return string.Equals(
                route.ShortName.Trim(),
                routeCode.Trim(),
                StringComparison.OrdinalIgnoreCase);
        }

        public static string Brand(this RouteItem route)
        {
            ArgumentNullException.ThrowIfNull(route);

            if (string.IsNullOrWhiteSpace(route.LongName))
                return route.ShortName.Trim().ToUpperInvariant();

            return route.LongName.Split('—')[0].Trim();
        }
    }
}
