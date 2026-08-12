using System.Linq;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using gmvTM.Domain.Infrastructure.Interfaces;
using gmvTM.Domain.Items;

namespace gmvTM.Server.Controllers.OData
{
    public sealed class StopTripItemsController : ODataController
    {
        private readonly IDatabaseContext _context;

        public StopTripItemsController(IDatabaseContext context) => _context = context;

        [EnableQuery]
        public IQueryable<StopTripItem> Get() => _context.StopTrips;
    }
}
