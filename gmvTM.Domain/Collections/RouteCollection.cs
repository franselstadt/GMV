using gmvTM.Domain.Collections.Base;
using gmvTM.Domain.Collections.Interfaces;
using gmvTM.Domain.Infrastructure.Persistence;
using gmvTM.Domain.Items;

namespace gmvTM.Domain.Collections
{
    public sealed class RouteCollection : BaseCollection<RouteItem>, IRouteCollection
    {
        public RouteCollection(DatabaseContext context) : base(context)
        {
        }
    }
}
