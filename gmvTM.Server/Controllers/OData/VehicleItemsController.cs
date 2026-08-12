using System.Linq;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using gmvTM.Domain.Infrastructure.Interfaces;
using gmvTM.Domain.Items;

namespace gmvTM.Server.Controllers.OData
{
    public sealed class VehicleItemsController : ODataController
    {
        private readonly IDatabaseContext _context;

        public VehicleItemsController(IDatabaseContext context) => _context = context;

        [EnableQuery]
        public IQueryable<VehicleItem> Get() => _context.Vehicles;
    }
}
