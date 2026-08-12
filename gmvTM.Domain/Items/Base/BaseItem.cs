using System;
using System.Threading;
using System.Threading.Tasks;
using gmvTM.Domain.Collections.Interfaces;
using gmvTM.Domain.Items.Interfaces;

namespace gmvTM.Domain.Items.Base
{
    public abstract class BaseItem : IBaseItem
    {
        private int id;

        public int ID
        {
            get { return this.id; }
            set { this.id = value; }
        }

        [View]
        public abstract string TableName
        {
            get;
        }

        [View]
        public bool IsNew
        {
            get { return this.ID == 0; }
        }

        #region revist
        public virtual void Create<TItem>(IBaseCollection<TItem> collection) where TItem : class, IItem
        {
            ArgumentNullException.ThrowIfNull(collection);

            TItem item = this as TItem ?? throw new InvalidOperationException( $"Item type '{this.GetType().Name}' cannot be created through '{typeof(TItem).Name}' collection.");
            
            collection.Create(item);
        }

        public virtual Task CreateAsync<TItem>(IBaseCollection<TItem> collection, CancellationToken cancellationToken = default) where TItem : class, IItem
        {
            ArgumentNullException.ThrowIfNull(collection);
            TItem item = this as TItem ?? throw new InvalidOperationException(
                    $"Item type '{this.GetType().Name}' cannot be created through '{typeof(TItem).Name}' collection.");
            
            return collection.CreateAsync(item, cancellationToken);
        }

        public static TItem? Read<TItem>(IBaseCollection<TItem> collection, int id) where TItem : class, IItem
        {
            ArgumentNullException.ThrowIfNull(collection);
            return collection.Read(id);
        }

        public static Task<TItem?> ReadAsync<TItem>(IBaseCollection<TItem> collection, int id, CancellationToken cancellationToken = default) where TItem : class, IItem
        {
            ArgumentNullException.ThrowIfNull(collection);
            return collection.ReadAsync(id, cancellationToken);
        }

        public virtual void Update<TItem>(IBaseCollection<TItem> collection) where TItem : class, IItem
        {
            ArgumentNullException.ThrowIfNull(collection);

            TItem item = this as TItem
                ?? throw new InvalidOperationException(
                    $"Item type '{this.GetType().Name}' cannot be " +
                    $"updated through '{typeof(TItem).Name}' collection.");
           
            collection.Update(item);
        }

        public virtual Task UpdateAsync<TItem>(IBaseCollection<TItem> collection, CancellationToken cancellationToken = default)  where TItem : class, IItem
        {
            ArgumentNullException.ThrowIfNull(collection);
            TItem item = this as TItem
                ?? throw new InvalidOperationException(
                    $"Item type '{this.GetType().Name}' cannot be updated through '{typeof(TItem).Name}' collection.");
            return collection.UpdateAsync(item, cancellationToken);
        }

        public virtual void Delete<TItem>(IBaseCollection<TItem> collection) where TItem : class, IItem
        {
            ArgumentNullException.ThrowIfNull(collection);
            collection.Delete(this.ID);
        }

        public virtual Task DeleteAsync<TItem>(IBaseCollection<TItem> collection, CancellationToken cancellationToken = default) where TItem : class, IItem
        {
            ArgumentNullException.ThrowIfNull(collection);
            return collection.DeleteAsync(this.ID, cancellationToken);
        }
        #endregion
    }
}
