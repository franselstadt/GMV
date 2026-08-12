using System;
using System.Collections.Generic;
using System.Linq;
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

            return stops.ReadItems()
                .Where(s => s.RouteID == routeID)
                .OrderBy(s => s.Sequence)
                .ThenBy(s => s.ID)
                .ToList();
        }

        public static async Task<IReadOnlyList<StopItem>> GetByRouteIDAsync(this IStopCollection stops, int routeID, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(stops);
            IReadOnlyList<StopItem> all = await stops.ReadItemsAsync(cancellationToken).ConfigureAwait(false);

            return all
                .Where(s => s.RouteID == routeID)
                .OrderBy(s => s.Sequence)
                .ThenBy(s => s.ID)
                .ToList();
        }

        public static HashSet<int> GetIDsByRouteID(this IStopCollection stops, int routeID)
        {
            ArgumentNullException.ThrowIfNull(stops);

            return stops.
                GetByRouteID(routeID).Select(s => s.ID).ToHashSet();
        }

        public static async Task<HashSet<int>> GetIDsByRouteIDAsync(this IStopCollection stops, int routeID, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(stops);

            IReadOnlyList<StopItem> rows = await stops.GetByRouteIDAsync(routeID, cancellationToken)
                .ConfigureAwait(false);

            return rows.Select(s => s.ID).ToHashSet();
        }

        public static StopItem? GetByCode(this IStopCollection stops, string? stopCode)
        {
            ArgumentNullException.ThrowIfNull(stops);

            if (string.IsNullOrWhiteSpace(stopCode))
                return null;

            string code = stopCode.Trim();

            return stops.ReadItems().FirstOrDefault(s =>
                string.Equals(s.StopCode.Trim(), code, StringComparison.OrdinalIgnoreCase));
        }

        public static async Task<StopItem?> GetByCodeAsync(this IStopCollection stops, string? stopCode, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(stops);

            if (string.IsNullOrWhiteSpace(stopCode))
                return null;

            string code = stopCode.Trim();
            IReadOnlyList<StopItem> all = await stops.ReadItemsAsync(cancellationToken).ConfigureAwait(false);

            return all.FirstOrDefault(s =>
                string.Equals(s.StopCode.Trim(), code, StringComparison.OrdinalIgnoreCase));
        }

        public static IReadOnlyList<StopItem> GetByIds(this IStopCollection stops, IEnumerable<int> ids)
        {
            ArgumentNullException.ThrowIfNull(stops);
            ArgumentNullException.ThrowIfNull(ids);

            HashSet<int> idSet = ids as HashSet<int> ?? ids.ToHashSet();

            if (!idSet.Any())
                return Array.Empty<StopItem>();

            return stops.ReadItems()
                .Where(s => idSet.Contains(s.ID))
                .OrderBy(s => s.Name, StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        public static async Task<IReadOnlyList<StopItem>> GetByIdsAsync(this IStopCollection stops, IEnumerable<int> ids, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(stops);
            ArgumentNullException.ThrowIfNull(ids);
            HashSet<int> idSet = ids as HashSet<int> ?? ids.ToHashSet();

            if (!idSet.Any())
                return Array.Empty<StopItem>();

            IReadOnlyList<StopItem> all = await stops.ReadItemsAsync(cancellationToken).ConfigureAwait(false);

            return all
                .Where(s => idSet.Contains(s.ID))
                .OrderBy(s => s.Name, StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        public static IReadOnlyList<StopItem> GetOnRouteOrderedBySequence(this IStopCollection stops, IReadOnlySet<int> stopIdsOnRoute)
        {
            ArgumentNullException.ThrowIfNull(stops);
            ArgumentNullException.ThrowIfNull(stopIdsOnRoute);

            if (!stopIdsOnRoute.Any())
                return Array.Empty<StopItem>();

            return stops.ReadItems()
                .Where(s => stopIdsOnRoute.Contains(s.ID))
                .OrderBy(s => s.Sequence)
                .ThenBy(s => s.ID)
                .ToList();
        }

        public static async Task<IReadOnlyList<StopItem>> GetOnRouteOrderedBySequenceAsync(this IStopCollection stops, IReadOnlySet<int> stopIdsOnRoute, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(stops);
            ArgumentNullException.ThrowIfNull(stopIdsOnRoute);

            if (!stopIdsOnRoute.Any())
                return Array.Empty<StopItem>();

            IReadOnlyList<StopItem> all = await stops.ReadItemsAsync(cancellationToken).ConfigureAwait(false);

            return all
                .Where(s => stopIdsOnRoute.Contains(s.ID))
                .OrderBy(s => s.Sequence)
                .ThenBy(s => s.ID)
                .ToList();
        }

        public static IReadOnlyList<StopItem> GetOnRouteOrderedByName(this IStopCollection stops, IReadOnlySet<int> stopIdsOnRoute)
        {
            ArgumentNullException.ThrowIfNull(stops);
            ArgumentNullException.ThrowIfNull(stopIdsOnRoute);

            return stops.GetByIds(stopIdsOnRoute);
        }

        public static Task<IReadOnlyList<StopItem>> GetOnRouteOrderedByNameAsync(this IStopCollection stops, IReadOnlySet<int> stopIdsOnRoute, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(stops);
            ArgumentNullException.ThrowIfNull(stopIdsOnRoute);

            return stops.GetByIdsAsync(stopIdsOnRoute, cancellationToken);
        }

        public static IReadOnlyDictionary<int, StopItem> GetLookupById(this IStopCollection stops)
        {
            ArgumentNullException.ThrowIfNull(stops);
            return stops.ReadItems().ToDictionary(s => s.ID);
        }

        public static async Task<IReadOnlyDictionary<int, StopItem>> GetLookupByIdAsync(this IStopCollection stops, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(stops);
            IReadOnlyList<StopItem> all = await stops.ReadItemsAsync(cancellationToken).ConfigureAwait(false);
            return all.ToDictionary(s => s.ID);
        }
    }
}
