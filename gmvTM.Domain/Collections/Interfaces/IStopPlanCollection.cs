using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using gmvTM.Domain.Items;

namespace gmvTM.Domain.Collections.Interfaces
{
    public interface IStopPlanCollection : IBaseCollection<StopPlanItem>
    {
        IReadOnlyList<StopPlanItem> ReadByRouteID(IStopCollection stops, int routeID);
        Task<IReadOnlyList<StopPlanItem>> ReadByRouteIDAsync(IStopCollection stops, int routeID, CancellationToken cancellationToken = default);
    }
}
