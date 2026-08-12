using System;
using System.Collections.Generic;
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
            return routes.ReadByCode(routeCode);
        }

        public static Task<RouteItem?> GetByCodeAsync(this IRouteCollection routes, string? routeCode, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(routes);
            return routes.ReadByCodeAsync(routeCode, cancellationToken);
        }

        public static IReadOnlyList<RouteItem> GetAllOrderedByShortName(this IRouteCollection routes)
        {
            ArgumentNullException.ThrowIfNull(routes);
            return routes.ReadAllOrderedByShortName();
        }

        public static Task<IReadOnlyList<RouteItem>> GetAllOrderedByShortNameAsync(this IRouteCollection routes, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(routes);
            return routes.ReadAllOrderedByShortNameAsync(cancellationToken);
        }
    }
}
