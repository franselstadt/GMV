using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using gmvTM.Domain.Items.Interfaces;

namespace gmvTM.Domain.Collections.Interfaces
{
    public interface IBaseCollection<TItem> where TItem : class, IItem
    {
        TItem? Read(int id);
        Task<TItem?> ReadAsync(int id, CancellationToken cancellationToken = default);
        IReadOnlyList<TItem> ReadItems();
        Task<IReadOnlyList<TItem>> ReadItemsAsync(CancellationToken cancellationToken = default);
        void Create(TItem item);
        Task CreateAsync(TItem item, CancellationToken cancellationToken = default);
        void CreateItems(IEnumerable<TItem> items);
        Task CreateItemsAsync(IEnumerable<TItem> items, CancellationToken cancellationToken = default);
        void Update(TItem item);
        Task UpdateAsync(TItem item, CancellationToken cancellationToken = default);
        void UpdateItems(IEnumerable<TItem> items);
        Task UpdateItemsAsync(IEnumerable<TItem> items, CancellationToken cancellationToken = default);
        void Delete(int id);
        Task DeleteAsync(int id, CancellationToken cancellationToken = default);
        void DeleteItems(IEnumerable<int> ids);
        Task DeleteItemsAsync(IEnumerable<int> ids, CancellationToken cancellationToken = default);
    }
}
