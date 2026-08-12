using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using gmvTM.Domain.Collections.Base;
using gmvTM.Domain.Collections.Interfaces;
using gmvTM.Domain.Infrastructure.Persistence;
using gmvTM.Domain.Items;

namespace gmvTM.Domain.Collections
{
    public sealed class StopCollection : BaseCollection<StopItem>, IStopCollection
    {
        public StopCollection(DatabaseContext context) : base(context)
        {
        }

        public IReadOnlyList<StopItem> ReadByRouteID(int routeID)
        {
            return this.ReadItems()
                .Where(s => s.RouteID == routeID)
                .OrderBy(s => s.Sequence)
                .ThenBy(s => s.ID)
                .ToList();
        }

        public async Task<IReadOnlyList<StopItem>> ReadByRouteIDAsync(int routeID, CancellationToken cancellationToken = default)
        {
            IReadOnlyList<StopItem> all = await this.ReadItemsAsync(cancellationToken).ConfigureAwait(false);

            return all
                .Where(s => s.RouteID == routeID)
                .OrderBy(s => s.Sequence)
                .ThenBy(s => s.ID)
                .ToList();
        }

        public HashSet<int> ReadIDsByRouteID(int routeID)
        {
            return this.ReadByRouteID(routeID).Select(s => s.ID).ToHashSet();
        }

        public async Task<HashSet<int>> ReadIDsByRouteIDAsync(int routeID, CancellationToken cancellationToken = default)
        {
            IReadOnlyList<StopItem> rows = await this.ReadByRouteIDAsync(routeID, cancellationToken).ConfigureAwait(false);
            return rows.Select(s => s.ID).ToHashSet();
        }

        public StopItem? ReadByCode(string? stopCode)
        {
            if (string.IsNullOrWhiteSpace(stopCode))
                return null;

            string code = stopCode.Trim();

            return this.ReadItems().FirstOrDefault(s =>
                string.Equals(s.StopCode.Trim(), code, StringComparison.OrdinalIgnoreCase));
        }

        public async Task<StopItem?> ReadByCodeAsync(string? stopCode, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(stopCode))
                return null;

            string code = stopCode.Trim();
            IReadOnlyList<StopItem> all = await this.ReadItemsAsync(cancellationToken).ConfigureAwait(false);

            return all.FirstOrDefault(s =>
                string.Equals(s.StopCode.Trim(), code, StringComparison.OrdinalIgnoreCase));
        }

        public IReadOnlyList<StopItem> ReadByIds(IEnumerable<int> ids)
        {
            ArgumentNullException.ThrowIfNull(ids);
            HashSet<int> idSet = ids as HashSet<int> ?? ids.ToHashSet();

            if (!idSet.Any())
                return Array.Empty<StopItem>();

            return this.ReadItems()
                .Where(s => idSet.Contains(s.ID))
                .OrderBy(s => s.Name, StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        public async Task<IReadOnlyList<StopItem>> ReadByIdsAsync(IEnumerable<int> ids, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(ids);
            HashSet<int> idSet = ids as HashSet<int> ?? ids.ToHashSet();

            if (!idSet.Any())
                return Array.Empty<StopItem>();

            IReadOnlyList<StopItem> all = await this.ReadItemsAsync(cancellationToken).ConfigureAwait(false);

            return all
                .Where(s => idSet.Contains(s.ID))
                .OrderBy(s => s.Name, StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        public IReadOnlyList<StopItem> ReadOnRouteOrderedBySequence(IReadOnlySet<int> stopIdsOnRoute)
        {
            ArgumentNullException.ThrowIfNull(stopIdsOnRoute);

            if (!stopIdsOnRoute.Any())
                return Array.Empty<StopItem>();

            return this.ReadItems()
                .Where(s => stopIdsOnRoute.Contains(s.ID))
                .OrderBy(s => s.Sequence)
                .ThenBy(s => s.ID)
                .ToList();
        }

        public async Task<IReadOnlyList<StopItem>> ReadOnRouteOrderedBySequenceAsync(IReadOnlySet<int> stopIdsOnRoute, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(stopIdsOnRoute);

            if (!stopIdsOnRoute.Any())
                return Array.Empty<StopItem>();

            IReadOnlyList<StopItem> all = await this.ReadItemsAsync(cancellationToken).ConfigureAwait(false);

            return all
                .Where(s => stopIdsOnRoute.Contains(s.ID))
                .OrderBy(s => s.Sequence)
                .ThenBy(s => s.ID)
                .ToList();
        }

        public IReadOnlyDictionary<int, StopItem> ReadLookupById()
        {
            return this.ReadItems().ToDictionary(s => s.ID);
        }

        public async Task<IReadOnlyDictionary<int, StopItem>> ReadLookupByIdAsync(CancellationToken cancellationToken = default)
        {
            IReadOnlyList<StopItem> all = await this.ReadItemsAsync(cancellationToken).ConfigureAwait(false);
            return all.ToDictionary(s => s.ID);
        }
    }
}
