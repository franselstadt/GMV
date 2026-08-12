using System;
using Xunit;
using FluentAssertions;
using gmvTM.Domain.Items;

namespace gmvTM.Domain.Tests
{
    public sealed class WhenCreatingItemsFromPhantoms
    {
        [Fact]
        public void ItShouldMapPhantomPropertiesByNameAndType()
        {
            StopItem stop = ItemFactory.CreateItem<StopItem>(new
            {
                RouteID = 1,
                StopCode = "F01",
                Name = "Depot",
                Latitude = 25.7,
                Longitude = -80.3,
                Sequence = 1,
                SpecialAlert = (string?)null
            });

            stop.RouteID.Should().Be(1);
            stop.StopCode.Should().Be("F01");
            stop.Name.Should().Be("Depot");
            stop.Latitude.Should().Be(25.7);
            stop.Longitude.Should().Be(-80.3);
            stop.Sequence.Should().Be(1);
            stop.SpecialAlert.Should().BeNull();
        }

        [Fact]
        public void ItShouldConvertCompatibleNumericTypes()
        {
            TripItem trip = ItemFactory.CreateItem<TripItem>(new { AverageMph = 20 });

            trip.AverageMph.Should().Be(20.0);
        }

        [Fact]
        public void ItShouldCarryUnmatchedDtoPropertiesAsDynamicProperties()
        {
            NextArrivalDto dto = ItemFactory.CreateItem<NextArrivalDto>(new
            {
                StopCode = "F01",
                HeadwaySeconds = 420
            });

            dto.StopCode.Should().Be("F01");
            dto.DynamicProperties.Should().ContainKey("HeadwaySeconds");
            dto.DynamicProperties["HeadwaySeconds"].Should().Be(420);
        }

        [Fact]
        public void ItShouldRejectPhantomPropertiesWithoutAMatchingName()
        {
            Func<RouteItem> act = () => ItemFactory.CreateItem<RouteItem>(new { ShortNameTypo = "F" });

            act.Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void ItShouldRejectPhantomPropertiesWithAnIncompatibleType()
        {
            Func<RouteItem> act = () => ItemFactory.CreateItem<RouteItem>(new { ShortName = new object() });

            act.Should().Throw<InvalidOperationException>();
        }
    }
}
