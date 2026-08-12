using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using gmvTM.Domain.Collections.Interfaces;
using gmvTM.Domain.Infrastructure.Persistence;
using gmvTM.Domain.Items.Interfaces;

namespace gmvTM.Domain.Collections.Base
{
    public abstract class BaseCollection<TItem> : IBaseCollection<TItem> where TItem : class, IItem
    {
        private readonly DatabaseContext context;

        protected BaseCollection(DatabaseContext context)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
        }

        protected DatabaseContext Context
        {
            get { return this.context; }
        }

        protected DbSet<TItem> Set
        {
            get { return this.Context.Set<TItem>(); }
        }

        public virtual TItem? Read(int id)
        {
            return this.Set.AsNoTracking().FirstOrDefault(x => x.ID == id);
        }

        public virtual async Task<TItem?> ReadAsync(int id, CancellationToken cancellationToken = default)
        {
            return await this.Set.AsNoTracking()
                .FirstOrDefaultAsync(x => x.ID == id, cancellationToken)
                .ConfigureAwait(false);
        }

        public virtual IReadOnlyList<TItem> ReadItems()
        {
            return this.Set.AsNoTracking().OrderBy(x => x.ID).ToList();
        }

        public virtual async Task<IReadOnlyList<TItem>> ReadItemsAsync(CancellationToken cancellationToken = default)
        {
            return await this.Set.AsNoTracking()
                .OrderBy(x => x.ID)
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);
        }

        public virtual void Create(TItem item)
        {
            ArgumentNullException.ThrowIfNull(item);
            this.Set.Add(item);
        }

        public virtual async Task CreateAsync(TItem item, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(item);
            await this.Set.AddAsync(item, cancellationToken).ConfigureAwait(false);
        }

        public virtual void CreateItems(IEnumerable<TItem> items)
        {
            ArgumentNullException.ThrowIfNull(items);
            List<TItem> list = items.ToList();

            if (!list.Any())
                return;

            this.Set.AddRange(list);
        }

        public virtual async Task CreateItemsAsync(IEnumerable<TItem> items, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(items);
            List<TItem> list = items.ToList();

            if (!list.Any())
                return;

            await this.Set.AddRangeAsync(list, cancellationToken).ConfigureAwait(false);
        }

        public virtual void Update(TItem item)
        {
            ArgumentNullException.ThrowIfNull(item);
            this.Set.Update(item);
        }

        public virtual Task UpdateAsync(TItem item, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(item);
            this.Set.Update(item);
            return Task.CompletedTask;
        }

        public virtual void UpdateItems(IEnumerable<TItem> items)
        {
            ArgumentNullException.ThrowIfNull(items);
            List<TItem> list = items.ToList();

            if (!list.Any())
                return;

            this.Set.UpdateRange(list);
        }

        public virtual Task UpdateItemsAsync(IEnumerable<TItem> items, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(items);
            List<TItem> list = items.ToList();

            if (!list.Any())
                return Task.CompletedTask;

            this.Set.UpdateRange(list);
            return Task.CompletedTask;
        }

        public virtual void Delete(int id)
        {
            TItem existing = this.Set.FirstOrDefault(x => x.ID == id);
            if (existing is null)
                return;

            this.Set.Remove(existing);
        }

        public virtual async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            TItem existing = await this.Set
                .FirstOrDefaultAsync(x => x.ID == id, cancellationToken)
                .ConfigureAwait(false);

            if (existing is null)
                return;

            this.Set.Remove(existing);
        }

        public virtual void DeleteItems(IEnumerable<int> ids)
        {
            ArgumentNullException.ThrowIfNull(ids);
            HashSet<int> idSet = ids as HashSet<int> ?? ids.ToHashSet();

            if (!idSet.Any())
                return;

            List<TItem> existing = this.Set.Where(x => idSet.Contains(x.ID)).ToList();
            if (!existing.Any())
                return;

            this.Set.RemoveRange(existing);
        }

        public virtual async Task DeleteItemsAsync(IEnumerable<int> ids, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(ids);
            HashSet<int> idSet = ids as HashSet<int> ?? ids.ToHashSet();

            if (!idSet.Any())
                return;

            List<TItem> existing = await this.Set
                .Where(x => idSet.Contains(x.ID))
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);

            if (!existing.Any())
                return;

            this.Set.RemoveRange(existing);
        }
    }
}
