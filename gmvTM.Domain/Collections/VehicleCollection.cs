using gmvTM.Domain.Collections.Base;
using gmvTM.Domain.Collections.Interfaces;
using gmvTM.Domain.Infrastructure.Persistence;
using gmvTM.Domain.Items;

namespace gmvTM.Domain.Collections
{
    public sealed class VehicleCollection : BaseCollection<VehicleItem>, IVehicleCollection
    {
        public VehicleCollection(DatabaseContext context) : base(context)
        {
        }
    }
}
