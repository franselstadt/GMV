using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using gmvTM.Domain.Infrastructure;
using gmvTM.Domain.Items;
using gmvTM.Domain.Strategies.ORM.EFCore.Workers;

namespace gmvTM.Domain.Strategies.ORM.EFCore.Infrastructure
{
    public sealed class DatabaseContext : DbContext, IDatabaseContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        {
        }

        public DbSet<RouteItem> Routes
        {
            get { return this.Set<RouteItem>(); }
        }

        public DbSet<StopItem> Stops
        {
            get { return this.Set<StopItem>(); }
        }

        public DbSet<VehicleItem> Vehicles
        {
            get { return this.Set<VehicleItem>(); }
        }

        public DbSet<TripItem> Trips
        {
            get { return this.Set<TripItem>(); }
        }

        public DbSet<StopPlanItem> StopPlans
        {
            get { return this.Set<StopPlanItem>(); }
        }

        public DbSet<StopTripItem> StopTrips
        {
            get { return this.Set<StopTripItem>(); }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            new TableConfigurationWorker().Configure(modelBuilder);
            base.OnModelCreating(modelBuilder);
        }

        int IDatabaseContext.SaveChanges()
        {
            return this.SaveChanges();
        }

        Task<int> IDatabaseContext.SaveChangesAsync(CancellationToken cancellationToken)
        {
            return this.SaveChangesAsync(cancellationToken);
        }
    }
}
