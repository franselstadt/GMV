using System.Collections.Generic;
using gmvTM.Domain.Items.View;

namespace gmvTM.Domain.Workers.Interfaces
{
    public interface IRoutePathBuilderWorker : IWorker
    {
        public RoutePathViewItem Build(IReadOnlyList<CoordinatesViewItem> decodedPoints);
    }
}
