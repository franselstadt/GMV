using gmvTM.Domain.Items.View;
using System.Threading;
using System.Threading.Tasks;

namespace gmvTM.Domain.Workers.Interfaces
{
    public interface ITripPathCalculatorWorker : IWorker
    {
        public TripPathViewItem Calculate(int routeID, int runIndex);
        public Task<TripPathViewItem> CalculateAsync(int routeID, int runIndex, CancellationToken cancellationToken = default);
    }
}
