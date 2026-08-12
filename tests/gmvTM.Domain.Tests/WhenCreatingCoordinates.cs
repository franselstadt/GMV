using System;
using Xunit;
using FluentAssertions;
using gmvTM.Domain.Items.View;

namespace gmvTM.Domain.Tests
{
    public sealed class WhenCreatingCoordinates
    {
        [Theory]
        [InlineData(-90.1, 0)]
        [InlineData(90.1, 0)]
        [InlineData(0, -180.1)]
        [InlineData(0, 180.1)]
        public void ItShouldRejectOutOfRangeValues(double latitude, double longitude)
        {
            Func<CoordinatesViewItem> act = () => new CoordinatesViewItem(latitude, longitude);

            act.Should().Throw<ArgumentOutOfRangeException>();
        }
    }
}
