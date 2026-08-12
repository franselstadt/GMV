using System;
using System.Collections.Generic;
using gmvTM.Domain.Items.View;
using gmvTM.Domain.Workers.Interfaces;

namespace gmvTM.Domain.Workers
{
    public sealed class TripPositionCalculatorWorker : ITripPositionCalculatorWorker
    {
        //claude assisted me in writing the math behind this method
        public VehicleMotionViewItem Calculate(RoutePathViewItem path, IReadOnlyList<PathStopViewItem> stopsInOrder, int startStopIndex, double speedMetersPerSecond, int averageDwellSeconds, int announceLeadSeconds, int doorClosingSeconds, TimeSpan elapsed)
        {
            ArgumentNullException.ThrowIfNull(path);
            ArgumentNullException.ThrowIfNull(stopsInOrder);
            ValidateTripInputs(stopsInOrder, startStopIndex, speedMetersPerSecond, averageDwellSeconds, doorClosingSeconds);

            if (announceLeadSeconds < 0)
                throw new ArgumentOutOfRangeException(nameof(announceLeadSeconds));

            double remaining = Math.Max(0, elapsed.TotalSeconds);
            PathStopViewItem last = stopsInOrder[stopsInOrder.Count - 1];

            for (int i = startStopIndex; i < stopsInOrder.Count - 1; i++)
            {
                PathStopViewItem from = stopsInOrder[i];
                PathStopViewItem to = stopsInOrder[i + 1];

                double legMeters = Math.Max(0, to.DistanceAlongPathMeters - from.DistanceAlongPathMeters);
                double travelSeconds = speedMetersPerSecond <= 0 ? 0 : legMeters / speedMetersPerSecond;

                if (remaining < travelSeconds)
                {
                    double traveled = remaining * speedMetersPerSecond;
                    
                    CoordinatesViewItem position = path.PointAtDistance(from.DistanceAlongPathMeters + traveled);
                    double secondsToStop = travelSeconds - remaining;
                    string phase = secondsToStop <= announceLeadSeconds
                        ? gmvDomain.VehiclePhases.Approaching
                        : gmvDomain.VehiclePhases.Traveling;

                    return new VehicleMotionViewItem(
                        position,
                        phase,
                        to.StopCode,
                        to.Name,
                        secondsToStop);
                }

                remaining -= travelSeconds;

                int doorsOpenSeconds = averageDwellSeconds - doorClosingSeconds;
                if (remaining < doorsOpenSeconds)
                {
                    return new VehicleMotionViewItem(
                        path.PointAtDistance(to.DistanceAlongPathMeters),
                        gmvDomain.VehiclePhases.DoorsOpen,
                        to.StopCode,
                        to.Name,
                        0);
                }

                remaining -= doorsOpenSeconds;

                if (remaining < doorClosingSeconds)
                {
                    return new VehicleMotionViewItem(
                        path.PointAtDistance(to.DistanceAlongPathMeters),
                        gmvDomain.VehiclePhases.DoorsClosing,
                        to.StopCode,
                        to.Name,
                        0);
                }

                remaining -= doorClosingSeconds;
            }

            return new VehicleMotionViewItem(
                path.PointAtDistance(last.DistanceAlongPathMeters),
                gmvDomain.VehiclePhases.Completed,
                last.StopCode,
                last.Name,
                0);
        }

        public double? SecondsUntilArrivalAtStop(IReadOnlyList<PathStopViewItem> stopsInOrder, int startStopIndex, double speedMetersPerSecond, int averageDwellSeconds, int doorClosingSeconds, TimeSpan elapsed, string targetStopCode)
        {
            ArgumentNullException.ThrowIfNull(stopsInOrder);
            ArgumentException.ThrowIfNullOrWhiteSpace(targetStopCode);
            ValidateTripInputs(stopsInOrder, startStopIndex, speedMetersPerSecond, averageDwellSeconds, doorClosingSeconds);

            string target = targetStopCode.Trim();
            double tripTime = 0;
            double elapsedSeconds = Math.Max(0, elapsed.TotalSeconds);
            int index = startStopIndex;

            for (int step = 0; step < 2 * (stopsInOrder.Count - 1); step++)
            {
                PathStopViewItem from = stopsInOrder[index];
                PathStopViewItem to = stopsInOrder[index + 1];

                double legMeters = Math.Max(0, to.DistanceAlongPathMeters - from.DistanceAlongPathMeters);
                double travelSeconds = speedMetersPerSecond <= 0 ? 0 : legMeters / speedMetersPerSecond;

                double arrivalAtTo = tripTime + travelSeconds;
                if (string.Equals(to.StopCode, target, StringComparison.OrdinalIgnoreCase))
                {
                    double remaining = arrivalAtTo - elapsedSeconds;
                    if (remaining >= -averageDwellSeconds)
                        return Math.Max(0, remaining);
                }

                tripTime = arrivalAtTo + averageDwellSeconds;

                index++;
                if (index == stopsInOrder.Count - 1)
                    index = 0;
            }

            return null;
        }

        private static void ValidateTripInputs(IReadOnlyList<PathStopViewItem> stopsInOrder, int startStopIndex, double speedMetersPerSecond, int averageDwellSeconds, int doorClosingSeconds)
        {
            if (stopsInOrder.Count < 2)
                throw new ArgumentException(gmvDomain.Messages.TripNeedsTwoStops, nameof(stopsInOrder));

            if (startStopIndex < 0 || startStopIndex >= stopsInOrder.Count - 1)
                throw new ArgumentOutOfRangeException(nameof(startStopIndex));

            if (speedMetersPerSecond <= 0)
                throw new ArgumentOutOfRangeException(nameof(speedMetersPerSecond));

            if (averageDwellSeconds < 2)
                throw new ArgumentOutOfRangeException(nameof(averageDwellSeconds));

            if (doorClosingSeconds < 1 || doorClosingSeconds >= averageDwellSeconds)
                throw new ArgumentOutOfRangeException(nameof(doorClosingSeconds));
        }
    }
}
