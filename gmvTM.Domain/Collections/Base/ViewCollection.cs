using System;
using System.Collections.Generic;
using gmvTM.Domain.Collections.Interfaces;
using gmvTM.Domain.Items.Interfaces;

namespace gmvTM.Domain.Collections.Base
{
    public abstract class ViewCollection<TViewItem> : IViewCollection<TViewItem> where TViewItem : class, IViewItem
    {
        private readonly List<TViewItem> items;

        protected ViewCollection()
        {
            this.items = new List<TViewItem>();
        }

        protected ViewCollection(IEnumerable<TViewItem> items)
        {
            ArgumentNullException.ThrowIfNull(items);
            this.items = new List<TViewItem>(items);
        }

        public IReadOnlyList<TViewItem> Items
        {
            get { return this.items; }
        }

        public int Count
        {
            get { return this.items.Count; }
        }

        public virtual void Add(TViewItem item)
        {
            ArgumentNullException.ThrowIfNull(item);
            this.items.Add(item);
        }

        public virtual void AddRange(IEnumerable<TViewItem> itemsToAdd)
        {
            ArgumentNullException.ThrowIfNull(itemsToAdd);
            this.items.AddRange(itemsToAdd);
        }

        public virtual bool Remove(TViewItem item)
        {
            ArgumentNullException.ThrowIfNull(item);
            return this.items.Remove(item);
        }

        public virtual void Clear()
        {
            this.items.Clear();
        }
    }
}
