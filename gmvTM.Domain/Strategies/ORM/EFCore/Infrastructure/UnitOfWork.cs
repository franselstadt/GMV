using System;
using System.Threading;
using System.Threading.Tasks;
using gmvTM.Domain.Collections.Interfaces;
using gmvTM.Domain.Infrastructure.Interfaces;

namespace gmvTM.Domain.Strategies.ORM.EFCore.Infrastructure
{
    public sealed class UnitOfWork : ISimpleUnitOfWork
    {
        private readonly DatabaseContext context;
        private readonly IRouteCollection routes;
        private readonly IStopCollection stops;
        private readonly IVehicleCollection vehicles;
        private readonly ITripCollection trips;
        private readonly IStopPlanCollection stopPlans;
        private readonly IStopTripCollection stopTrips;

        public UnitOfWork(DatabaseContext context, IRouteCollection routes, IStopCollection stops, IVehicleCollection vehicles, ITripCollection trips, IStopPlanCollection stopPlans, IStopTripCollection stopTrips)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
            this.routes = routes ?? throw new ArgumentNullException(nameof(routes));
            this.stops = stops ?? throw new ArgumentNullException(nameof(stops));
            this.vehicles = vehicles ?? throw new ArgumentNullException(nameof(vehicles));
            this.trips = trips ?? throw new ArgumentNullException(nameof(trips));
            this.stopPlans = stopPlans
                ?? throw new ArgumentNullException(nameof(stopPlans));
            this.stopTrips = stopTrips ?? throw new ArgumentNullException(nameof(stopTrips));
        }

        public IRouteCollection Routes
        {
            get { return this.routes; }
        }

        public IStopCollection Stops
        {
            get { return this.stops; }
        }

        public IVehicleCollection Vehicles
        {
            get { return this.vehicles; }
        }

        public ITripCollection Trips
        {
            get { return this.trips; }
        }

        public IStopPlanCollection StopPlans
        {
            get { return this.stopPlans; }
        }

        public IStopTripCollection StopTrips
        {
            get { return this.stopTrips; }
        }

        public int SaveChanges()
        {
            return this.context.SaveChanges();
        }

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return this.context.SaveChangesAsync(cancellationToken);
        }
    }
}
