using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using gmvTM.Domain.Items;

namespace gmvTM.Domain.Collections.Interfaces
{
    public interface IRouteCollection : IBaseCollection<RouteItem>
    {
        RouteItem? ReadByCode(string? routeCode);
        Task<RouteItem?> ReadByCodeAsync(string? routeCode, CancellationToken cancellationToken = default);
        IReadOnlyList<RouteItem> ReadAllOrderedByShortName();
        Task<IReadOnlyList<RouteItem>> ReadAllOrderedByShortNameAsync(CancellationToken cancellationToken = default);
    }
}
