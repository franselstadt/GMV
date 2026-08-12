using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using gmvTM.Domain.Collections.Interfaces;
using gmvTM.Domain.Items;

namespace gmvTM.Domain.Extensions.Collections
{
    public static class StopCollectionExtensions
    {
        public static IReadOnlyList<StopItem> GetByRouteID(this IStopCollection stops, int routeID)
        {
            ArgumentNullException.ThrowIfNull(stops);
            return stops.ReadByRouteID(routeID);
        }

        public static Task<IReadOnlyList<StopItem>> GetByRouteIDAsync(this IStopCollection stops, int routeID, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(stops);
            return stops.ReadByRouteIDAsync(routeID, cancellationToken);
        }

        public static HashSet<int> GetIDsByRouteID(this IStopCollection stops, int routeID)
        {
            ArgumentNullException.ThrowIfNull(stops);
            return stops.ReadIDsByRouteID(routeID);
        }

        public static Task<HashSet<int>> GetIDsByRouteIDAsync(this IStopCollection stops, int routeID, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(stops);
            return stops.ReadIDsByRouteIDAsync(routeID, cancellationToken);
        }

        public static StopItem? GetByCode(this IStopCollection stops, string? stopCode)
        {
            ArgumentNullException.ThrowIfNull(stops);
            return stops.ReadByCode(stopCode);
        }

        public static Task<StopItem?> GetByCodeAsync(this IStopCollection stops, string? stopCode, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(stops);
            return stops.ReadByCodeAsync(stopCode, cancellationToken);
        }

        public static IReadOnlyList<StopItem> GetByIds(this IStopCollection stops, IEnumerable<int> ids)
        {
            ArgumentNullException.ThrowIfNull(stops);
            return stops.ReadByIds(ids);
        }

        public static Task<IReadOnlyList<StopItem>> GetByIdsAsync(this IStopCollection stops, IEnumerable<int> ids, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(stops);
            return stops.ReadByIdsAsync(ids, cancellationToken);
        }

        public static IReadOnlyList<StopItem> GetOnRouteOrderedBySequence(this IStopCollection stops, IReadOnlySet<int> stopIdsOnRoute)
        {
            ArgumentNullException.ThrowIfNull(stops);
            return stops.ReadOnRouteOrderedBySequence(stopIdsOnRoute);
        }

        public static Task<IReadOnlyList<StopItem>> GetOnRouteOrderedBySequenceAsync(this IStopCollection stops, IReadOnlySet<int> stopIdsOnRoute, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(stops);
            return stops.ReadOnRouteOrderedBySequenceAsync(stopIdsOnRoute, cancellationToken);
        }

        public static IReadOnlyList<StopItem> GetOnRouteOrderedByName(this IStopCollection stops, IReadOnlySet<int> stopIdsOnRoute)
        {
            ArgumentNullException.ThrowIfNull(stops);
            return stops.ReadByIds(stopIdsOnRoute);
        }

        public static Task<IReadOnlyList<StopItem>> GetOnRouteOrderedByNameAsync(this IStopCollection stops, IReadOnlySet<int> stopIdsOnRoute, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(stops);
            return stops.ReadByIdsAsync(stopIdsOnRoute, cancellationToken);
        }

        public static IReadOnlyDictionary<int, StopItem> GetLookupById(this IStopCollection stops)
        {
            ArgumentNullException.ThrowIfNull(stops);
            return stops.ReadLookupById();
        }

        public static Task<IReadOnlyDictionary<int, StopItem>> GetLookupByIdAsync(this IStopCollection stops, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(stops);
            return stops.ReadLookupByIdAsync(cancellationToken);
        }
    }
}
