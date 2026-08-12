using System.Linq;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using gmvTM.Domain.Infrastructure.Interfaces;
using gmvTM.Domain.Items;

namespace gmvTM.Server.Controllers.OData
{
    public sealed class StopItemsController : ODataController
    {
        private readonly IDatabaseContext _context;

        public StopItemsController(IDatabaseContext context) => _context = context;

        [EnableQuery]
        public IQueryable<StopItem> Get() => _context.Stops;
    }
}
