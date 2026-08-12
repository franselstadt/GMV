using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using gmvTM.Domain.Items;

namespace gmvTM.Domain.Infrastructure.Persistence
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
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(DatabaseContext).Assembly);
            IgnoreViewProperties(modelBuilder);
            base.OnModelCreating(modelBuilder);
        }

        private static void IgnoreViewProperties(ModelBuilder modelBuilder)
        {
            foreach (IMutableEntityType entityType in modelBuilder.Model.GetEntityTypes())
            {
                Type clrType = entityType.ClrType;
                foreach (PropertyInfo property in clrType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
                {
                    if (property.GetCustomAttribute<ViewAttribute>(inherit: true) is null)
                        continue;

                    modelBuilder.Entity(clrType).Ignore(property.Name);
                }
            }
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
