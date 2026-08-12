using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using gmvTM.Domain.Collections.Interfaces;
using gmvTM.Domain.Items;

namespace gmvTM.Domain.Extensions.Collections
{
    public static class RouteCollectionExtensions
    {
        public static RouteItem? GetByCode(this IRouteCollection routes, string? routeCode)
        {
            ArgumentNullException.ThrowIfNull(routes);

            if (string.IsNullOrWhiteSpace(routeCode))
                return null;

            string code = routeCode.Trim();

            return routes.ReadItems().FirstOrDefault(r =>
                string.Equals(r.ShortName.Trim(), code, StringComparison.OrdinalIgnoreCase));
        }

        public static async Task<RouteItem?> GetByCodeAsync(this IRouteCollection routes, string? routeCode, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(routes);
            if (string.IsNullOrWhiteSpace(routeCode))
                return null;

            string code = routeCode.Trim();

            IReadOnlyList<RouteItem> all = await routes.ReadItemsAsync(cancellationToken).ConfigureAwait(false);

            return all.FirstOrDefault(r =>
                string.Equals(r.ShortName.Trim(), code, StringComparison.OrdinalIgnoreCase));
        }

        public static IReadOnlyList<RouteItem> GetAllOrderedByShortName(this IRouteCollection routes)
        {
            ArgumentNullException.ThrowIfNull(routes);

            return routes.ReadItems()
                .OrderBy(r => r.ShortName, StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        public static async Task<IReadOnlyList<RouteItem>> GetAllOrderedByShortNameAsync(this IRouteCollection routes, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(routes);
            IReadOnlyList<RouteItem> all = await routes.ReadItemsAsync(cancellationToken).ConfigureAwait(false);
           
             return all.OrderBy(r => r.ShortName, StringComparer.OrdinalIgnoreCase).ToList();
        }
    }
}
