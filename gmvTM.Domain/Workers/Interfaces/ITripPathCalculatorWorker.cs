using gmvTM.Domain.Items.View;
using System.Threading;
using System.Threading.Tasks;

namespace gmvTM.Domain.Workers.Interfaces
{
    public interface ITripPathCalculatorWorker : IWorker
    {
        public TripPathViewItem Calculate(int routeID);
        public Task<TripPathViewItem> CalculateAsync(int routeID, CancellationToken cancellationToken = default);
    }
}
