using gmvTM.Domain.Collections.Base;
using gmvTM.Domain.Collections.Interfaces;
using gmvTM.Domain.Infrastructure.Persistence;
using gmvTM.Domain.Items;

namespace gmvTM.Domain.Collections
{
    public sealed class TripCollection : BaseCollection<TripItem>, ITripCollection
    {
        public TripCollection(DatabaseContext context) : base(context)
        {
        }
    }
}
