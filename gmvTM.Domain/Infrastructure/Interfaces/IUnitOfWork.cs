using System.Threading;
using System.Threading.Tasks;
using gmvTM.Domain.Collections.Interfaces;

namespace gmvTM.Domain.Infrastructure.Interfaces
{
    public interface ISimpleUnitOfWork
    {
        IRouteCollection Routes
        {
            get;
        }

        IStopCollection Stops
        {
            get;
        }

        IVehicleCollection Vehicles
        {
            get;
        }

        ITripCollection Trips
        {
            get;
        }

        IStopPlanCollection StopPlans
        {
            get;
        }

        IStopTripCollection StopTrips
        {
            get;
        }

        int SaveChanges();

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
