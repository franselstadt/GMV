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
    }
}
