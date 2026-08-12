using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using gmvTM.Domain.Items;

namespace gmvTM.Domain.Infrastructure
{
    public interface IDatabaseContext
    {
        DbSet<RouteItem> Routes
        {
            get;
        }

        DbSet<StopItem> Stops
        {
            get;
        }

        DbSet<VehicleItem> Vehicles
        {
            get;
        }

        DbSet<TripItem> Trips
        {
            get;
        }

        DbSet<StopPlanItem> StopPlans
        {
            get;
        }

        DbSet<StopTripItem> StopTrips
        {
            get;
        }

        int SaveChanges();

        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
