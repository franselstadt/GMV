using gmvTM.Domain.Items.View;
using System;
using System.Collections.Generic;

namespace gmvTM.Domain.Workers.Interfaces
{
    public interface ITripPositionCalculatorWorker : IWorker
    {
        public VehicleMotionViewItem Calculate(RoutePathViewItem path, IReadOnlyList<PathStopViewItem> stopsInOrder, int startStopIndex, double speedMetersPerSecond, int averageDwellSeconds, int announceLeadSeconds, int doorClosingSeconds, TimeSpan elapsed);
        public double? SecondsUntilArrivalAtStop(IReadOnlyList<PathStopViewItem> stopsInOrder, int startStopIndex, double speedMetersPerSecond, int averageDwellSeconds, int doorClosingSeconds, TimeSpan elapsed, string targetStopCode);
    }
}
