using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace gmvTM.Domain.Workers.Interfaces
{
    public interface INextArrivalsCalculatorWorker : IWorker
    {
        public IReadOnlyList<NextArrivalDto> Calculate(int routeID, int stopID, string stopCode, int count, DateTime agencyNow);
        public Task<IReadOnlyList<NextArrivalDto>> CalculateAsync(int routeID, int stopID, string stopCode, int count, DateTime agencyNow, CancellationToken cancellationToken = default);
    }
}
