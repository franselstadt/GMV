using gmvTM.Domain.Items.Interfaces;
using System.Collections.Generic;

namespace gmvTM.Domain.Collections.Interfaces
{
    public interface IViewCollection<TViewItem> where TViewItem : class, IViewItem
    {
        IReadOnlyList<TViewItem> Items { get; }
        int Count { get; }

        void Add(TViewItem item);
        void AddRange(IEnumerable<TViewItem> items);
        bool Remove(TViewItem item);
        void Clear();
    }
}
