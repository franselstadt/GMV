using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using gmvTM.Domain.Items;

namespace gmvTM.Domain.Collections.Interfaces
{
    public interface IStopCollection : IBaseCollection<StopItem>
    {
        IReadOnlyList<StopItem> ReadByRouteID(int routeID);
        Task<IReadOnlyList<StopItem>> ReadByRouteIDAsync(int routeID, CancellationToken cancellationToken = default);
        HashSet<int> ReadIDsByRouteID(int routeID);
        Task<HashSet<int>> ReadIDsByRouteIDAsync(int routeID, CancellationToken cancellationToken = default);
        StopItem? ReadByCode(string? stopCode);
        Task<StopItem?> ReadByCodeAsync(string? stopCode, CancellationToken cancellationToken = default);
        IReadOnlyList<StopItem> ReadByIds(IEnumerable<int> ids);
        Task<IReadOnlyList<StopItem>> ReadByIdsAsync(IEnumerable<int> ids, CancellationToken cancellationToken = default);
        IReadOnlyList<StopItem> ReadOnRouteOrderedBySequence(IReadOnlySet<int> stopIdsOnRoute);
        Task<IReadOnlyList<StopItem>> ReadOnRouteOrderedBySequenceAsync(IReadOnlySet<int> stopIdsOnRoute, CancellationToken cancellationToken = default);
        IReadOnlyDictionary<int, StopItem> ReadLookupById();
        Task<IReadOnlyDictionary<int, StopItem>> ReadLookupByIdAsync(CancellationToken cancellationToken = default);
    }
}
