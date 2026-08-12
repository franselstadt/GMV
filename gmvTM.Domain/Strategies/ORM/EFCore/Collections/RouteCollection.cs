using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using gmvTM.Domain.Collections.Interfaces;
using gmvTM.Domain.Strategies.ORM.EFCore.Infrastructure;
using gmvTM.Domain.Items;

namespace gmvTM.Domain.Strategies.ORM.EFCore.Collections
{
    public sealed class RouteCollection : BaseCollection<RouteItem>, IRouteCollection
    {
        public RouteCollection(DatabaseContext context) : base(context)
        {
        }

        public RouteItem? ReadByCode(string? routeCode)
        {
            if (string.IsNullOrWhiteSpace(routeCode))
                return null;

            string code = routeCode.Trim();

            return this.ReadItems().FirstOrDefault(r =>
                string.Equals(r.ShortName.Trim(), code, StringComparison.OrdinalIgnoreCase));
        }

        public async Task<RouteItem?> ReadByCodeAsync(string? routeCode, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(routeCode))
                return null;

            string code = routeCode.Trim();
            IReadOnlyList<RouteItem> all = await this.ReadItemsAsync(cancellationToken).ConfigureAwait(false);

            return all.FirstOrDefault(r =>
                string.Equals(r.ShortName.Trim(), code, StringComparison.OrdinalIgnoreCase));
        }

        public IReadOnlyList<RouteItem> ReadAllOrderedByShortName()
        {
            return this.ReadItems()
                .OrderBy(r => r.ShortName, StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        public async Task<IReadOnlyList<RouteItem>> ReadAllOrderedByShortNameAsync(CancellationToken cancellationToken = default)
        {
            IReadOnlyList<RouteItem> all = await this.ReadItemsAsync(cancellationToken).ConfigureAwait(false);
            return all.OrderBy(r => r.ShortName, StringComparer.OrdinalIgnoreCase).ToList();
        }
    }
}
