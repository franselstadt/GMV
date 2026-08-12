using System.Linq;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using gmvTM.Domain.Infrastructure.Interfaces;
using gmvTM.Domain.Items;

namespace gmvTM.Server.Controllers.OData
{
    public sealed class TripItemsController : ODataController
    {
        private readonly IDatabaseContext _context;

        public TripItemsController(IDatabaseContext context) => _context = context;

        [EnableQuery]
        public IQueryable<TripItem> Get() => _context.Trips;
    }
}
