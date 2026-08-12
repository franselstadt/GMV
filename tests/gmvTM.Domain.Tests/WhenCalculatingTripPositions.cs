using Xunit;
using FluentAssertions;
using System;
using System.Collections.Generic;
using gmvTM.Domain;
using gmvTM.Domain.Items.View;
using gmvTM.Domain.Workers;

namespace gmvTM.Domain.Tests
{
    public sealed class WhenCalculatingTripPositions
    {
        private readonly RoutePathViewItem _path;
        private readonly IReadOnlyList<PathStopViewItem> _stops;
        private readonly TripPositionCalculatorWorker _calculator = new();

        public WhenCalculatingTripPositions()
        {
            List<CoordinatesViewItem> points =
            [
                new CoordinatesViewItem(34.0000, -118.2500),
                new CoordinatesViewItem(34.0010, -118.2500),
                new CoordinatesViewItem(34.0020, -118.2500)
            ];
            _path = new RoutePathBuilderWorker().Build(points);
            _stops =
            [
                new PathStopViewItem(1, "A", "Stop A", 0, 0),
                new PathStopViewItem(2, "B", "Stop B", _path.TotalMeters / 2, 300),
                new PathStopViewItem(3, "C", "Stop C", _path.TotalMeters, 600)
            ];
        }

        [Fact]
        public void ItShouldInterpolateWhileTraveling()
        {
            double halfLegSeconds = (_stops[1].DistanceAlongPathMeters - _stops[0].DistanceAlongPathMeters) / 10.0 / 2.0;

            VehicleMotionViewItem motion = _calculator.Calculate(
                _path,
                _stops,
                startStopIndex: 0,
                speedMetersPerSecond: 10,
                averageDwellSeconds: 12,
                announceLeadSeconds: 1,
                doorClosingSeconds: 4,
                elapsed: TimeSpan.FromSeconds(halfLegSeconds));

            motion.Phase.Should().Be(gmvDomain.VehiclePhases.Traveling);
            motion.StopCode.Should().Be("B");
            motion.Position.Latitude.Should().BeGreaterThan(34.0000);
            motion.Position.Latitude.Should().BeLessThan(34.0010);
        }

        [Fact]
        public void ItShouldAnnounceWhenWithinLeadTime()
        {
            double travelSeconds = (_stops[1].DistanceAlongPathMeters - _stops[0].DistanceAlongPathMeters) / 10.0;

            VehicleMotionViewItem motion = _calculator.Calculate(
                _path,
                _stops,
                startStopIndex: 0,
                speedMetersPerSecond: 10,
                averageDwellSeconds: 12,
                announceLeadSeconds: 30,
                doorClosingSeconds: 4,
                elapsed: TimeSpan.FromSeconds(Math.Max(0, travelSeconds - 5)));

            motion.Phase.Should().Be(gmvDomain.VehiclePhases.Approaching);
            motion.StopName.Should().Be("Stop B");
            motion.SecondsToStop.Should().BeApproximately(5, 0.5);
        }

        [Fact]
        public void ItShouldReturnZeroWhileDwellingAtTheRequestedStop()
        {
            double legSeconds = (_stops[1].DistanceAlongPathMeters - _stops[0].DistanceAlongPathMeters) / 10.0;

            double? seconds = _calculator.SecondsUntilArrivalAtStop(
                _stops,
                startStopIndex: 0,
                speedMetersPerSecond: 10,
                averageDwellSeconds: 12,
                doorClosingSeconds: 4,
                elapsed: TimeSpan.FromSeconds(legSeconds + 5),
                targetStopCode: "B");

            seconds.Should().Be(0);
        }

        [Fact]
        public void ItShouldEstimateTheNextTripWhenTheStopWasAlreadyPassed()
        {
            double legSeconds = (_stops[1].DistanceAlongPathMeters - _stops[0].DistanceAlongPathMeters) / 10.0;
            double elapsed = legSeconds + 20;
            double nextTripArrival = 3 * legSeconds + 24;

            double? seconds = _calculator.SecondsUntilArrivalAtStop(
                _stops,
                startStopIndex: 0,
                speedMetersPerSecond: 10,
                averageDwellSeconds: 12,
                doorClosingSeconds: 4,
                elapsed: TimeSpan.FromSeconds(elapsed),
                targetStopCode: "B");

            seconds.Should().NotBeNull();
            seconds!.Value.Should().BeApproximately(nextTripArrival - elapsed, 0.5);
        }

        [Fact]
        public void ItShouldReportDoorsOpenDuringDwell()
        {
            double travelSeconds = (_stops[1].DistanceAlongPathMeters - _stops[0].DistanceAlongPathMeters) / 10.0;

            VehicleMotionViewItem motion = _calculator.Calculate(
                _path,
                _stops,
                startStopIndex: 0,
                speedMetersPerSecond: 10,
                averageDwellSeconds: 12,
                announceLeadSeconds: 30,
                doorClosingSeconds: 4,
                elapsed: TimeSpan.FromSeconds(travelSeconds + 1));

            motion.Phase.Should().Be(gmvDomain.VehiclePhases.DoorsOpen);
            motion.StopCode.Should().Be("B");
        }
    }
}
