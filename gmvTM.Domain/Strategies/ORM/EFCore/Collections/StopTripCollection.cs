using gmvTM.Domain.Collections.Interfaces;
using gmvTM.Domain.Strategies.ORM.EFCore.Infrastructure;
using gmvTM.Domain.Items;

namespace gmvTM.Domain.Strategies.ORM.EFCore.Collections
{
    public sealed class StopTripCollection : BaseCollection<StopTripItem>, IStopTripCollection
    {
        public StopTripCollection(DatabaseContext context) : base(context)
        {
        }
    }
}
