using gmvTM.Domain.Collections.Base;
using gmvTM.Domain.Collections.Interfaces;
using gmvTM.Domain.Infrastructure.Persistence;
using gmvTM.Domain.Items;

namespace gmvTM.Domain.Collections
{
    public sealed class StopTripCollection : BaseCollection<StopTripItem>, IStopTripCollection
    {
        public StopTripCollection(DatabaseContext context) : base(context)
        {
        }
    }
}
