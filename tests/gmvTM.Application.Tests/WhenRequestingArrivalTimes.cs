using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using Xunit;
using gmvTM.Application.Classes.Exceptions;
using gmvTM.Application.Classes.Services;
using gmvTM.Application.Interfaces;
using gmvTM.Domain;
using gmvTM.Domain.Collections.Interfaces;
using gmvTM.Domain.Items;
using gmvTM.Domain.Workers.Interfaces;
using gmvTestConstants;

namespace gmvTM.Application.Tests
{
    public sealed class WhenRequestingArrivalTimes
    {
        [Fact]
        public async Task ItShouldRejectBlankStopCode()
        {
            RouteItem route = RouteFactory.CreateItem(gmvTest.Data.RouteCode, gmvTest.Data.RouteLongName, null, gmvTest.Data.EncodedPolyline, id: 1);

            IRouteCollection routes = Substitute.For<IRouteCollection>();
            routes.ReadByCodeAsync(Arg.Any<string?>(), Arg.Any<CancellationToken>())
                .Returns(route);

            RouteStopService sut = new RouteStopService(
                routes,
                Substitute.For<IStopCollection>(),
                Substitute.For<IStopPlanCollection>(),
                Substitute.For<IDateTimeProviderWorker>(),
                Substitute.For<ITripPositionCalculatorWorker>(),
                Substitute.For<ISimulationStore>());

            Func<Task> act = async () => await sut.GetNextArrivalsAsync(gmvTest.Data.RouteCode, "  ", CancellationToken.None);

            await act.Should().ThrowAsync<ValidationException>();
        }

        [Fact]
        public async Task ItShouldRequireAnActiveSimulation()
        {
            RouteItem route = RouteFactory.CreateItem(gmvTest.Data.RouteCode, gmvTest.Data.RouteLongName, null, gmvTest.Data.EncodedPolyline, id: 1);

            IRouteCollection routes = Substitute.For<IRouteCollection>();
            routes.ReadByCodeAsync(Arg.Any<string?>(), Arg.Any<CancellationToken>())
                .Returns(route);

            ISimulationStore store = Substitute.For<ISimulationStore>();
            store.ListActive().Returns(Array.Empty<gmvTM.Application.Classes.Simulation.ActiveSimulation>());

            RouteStopService sut = new RouteStopService(
                routes,
                Substitute.For<IStopCollection>(),
                Substitute.For<IStopPlanCollection>(),
                Substitute.For<IDateTimeProviderWorker>(),
                Substitute.For<ITripPositionCalculatorWorker>(),
                store);

            Func<Task> act = async () => await sut.GetNextArrivalsAsync(gmvTest.Data.RouteCode, gmvTest.Data.DepotStopCode, CancellationToken.None);

            await act.Should().ThrowAsync<ValidationException>()
                .WithMessage(gmvDomain.Messages.SimulationRequiredForArrivals);
        }

        [Fact]
        public async Task ItShouldReturnPlannedAndActualClockTimesFromScheduleSeconds()
        {
            RouteItem route = RouteFactory.CreateItem(gmvTest.Data.RouteCode, gmvTest.Data.RouteLongName, null, gmvTest.Data.EncodedPolyline, id: 1);
            StopItem start = StopFactory.CreateItem(1, gmvTest.Data.OriginStopCode, gmvTest.Data.OriginStopName, 25.7, -80.3, 0, id: 1);
            StopItem stop = StopFactory.CreateItem(1, gmvTest.Data.DepotStopCode, gmvTest.Data.DepotStopName, 25.7, -80.3, 1, id: 10);

            IRouteCollection routes = Substitute.For<IRouteCollection>();
            routes.ReadByCodeAsync(Arg.Any<string?>(), Arg.Any<CancellationToken>())
                .Returns(route);

            IStopCollection stops = Substitute.For<IStopCollection>();
            stops.ReadByCodeAsync(gmvTest.Data.DepotStopCode, Arg.Any<CancellationToken>())
                .Returns(stop);

            IStopPlanCollection scheduled = Substitute.For<IStopPlanCollection>();
            scheduled.ReadByRouteIDAsync(Arg.Any<IStopCollection>(), route.ID, Arg.Any<CancellationToken>())
                .Returns(new List<StopPlanItem>
                {
                    StopPlanFactory.CreateItem(1, 0, 0, id: 1),
                    StopPlanFactory.CreateItem(10, 1, 600, id: 2)
                });

            IDateTimeProviderWorker clock = Substitute.For<IDateTimeProviderWorker>();
            DateTime utcNow = new DateTime(2026, 8, 10, 16, 0, 0, DateTimeKind.Utc);
            clock.AgencyNow.Returns(new DateTime(2026, 8, 10, 9, 0, 0));
            clock.UtcNow.Returns(utcNow);

            ITripPositionCalculatorWorker positions = Substitute.For<ITripPositionCalculatorWorker>();
            positions.SecondsUntilArrivalAtStop(
                    Arg.Any<System.Collections.Generic.IReadOnlyList<gmvTM.Domain.Items.View.PathStopViewItem>>(),
                    Arg.Any<int>(),
                    Arg.Any<double>(),
                    Arg.Any<int>(),
                    Arg.Any<int>(),
                    Arg.Any<TimeSpan>(),
                    gmvTest.Data.DepotStopCode)
                .Returns(120.0);

            gmvTM.Application.Classes.Simulation.ActiveSimulation simulation =
                new gmvTM.Application.Classes.Simulation.ActiveSimulation(
                    id: 1,
                    routeCode: gmvTest.Data.RouteCode,
                    vehicleID: 1,
                    vehicleNumber: gmvTest.Data.VehicleNumber,
                    tripID: 1,
                    status: gmvDomain.Messages.SimulationStatusRunning,
                    startStopCode: gmvTest.Data.OriginStopCode,
                    averageMph: gmvTest.Data.UnitTestAverageMph,
                    averageDwellSeconds: gmvTest.Data.AverageDwellSeconds,
                    startedAtUtc: utcNow.AddSeconds(-60),
                    startStopIndex: 0,
                    speedMetersPerSecond: 8,
                    path: new gmvTM.Domain.Workers.RoutePathBuilderWorker().Build(
                    [
                        new gmvTM.Domain.Items.View.CoordinatesViewItem(34.0, -118.25),
                        new gmvTM.Domain.Items.View.CoordinatesViewItem(34.01, -118.25)
                    ]),
                    stops:
                    [
                        new gmvTM.Domain.Items.View.PathStopViewItem(1, gmvTest.Data.OriginStopCode, gmvTest.Data.OriginStopName, 0, 0),
                        new gmvTM.Domain.Items.View.PathStopViewItem(10, gmvTest.Data.DepotStopCode, gmvTest.Data.DepotStopName, 1000, 600)
                    ]);

            ISimulationStore store = Substitute.For<ISimulationStore>();
            store.ListActive().Returns(new[] { simulation });

            RouteStopService sut = new RouteStopService(
                routes,
                stops,
                scheduled,
                clock,
                positions,
                store);

            NextArrivalDto? result =
                await sut.GetNextArrivalsAsync(gmvTest.Data.RouteCode, gmvTest.Data.DepotStopCode, CancellationToken.None);

            result.Should().NotBeNull();
            result!.RunLabel.Should().Be("DASH F 08:59");
            result.PlannedTime.Should().Be(new TimeOnly(9, 9));
            result.ActualTime.Should().Be(new TimeOnly(9, 2));
            result.Status.Should().Be(gmvDomain.ScheduleStatuses.Ahead);
        }
    }
}
