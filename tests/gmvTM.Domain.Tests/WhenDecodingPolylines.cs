using System.Collections.Generic;
using FluentAssertions;
using Xunit;
using gmvTM.Domain.Items.View;
using gmvTM.Domain.Workers;

namespace gmvTM.Domain.Tests
{
    public sealed class WhenDecodingPolylines
    {
        [Fact]
        public void ItShouldDecodeAKnownEncodedString()
        {
            const string encoded = "_p~iF~ps|U_ulLnnqC_mqNvxq`@";
            PolylineDecoderWorker decoder = new PolylineDecoderWorker();

            List<CoordinatesViewItem> points = decoder.Decode(encoded);

            points.Should().HaveCount(3);
            points[0].Latitude.Should().BeApproximately(38.5, 0.00001);
            points[0].Longitude.Should().BeApproximately(-120.2, 0.00001);
            points[1].Latitude.Should().BeApproximately(40.7, 0.00001);
            points[1].Longitude.Should().BeApproximately(-120.95, 0.00001);
            points[2].Latitude.Should().BeApproximately(43.252, 0.00001);
            points[2].Longitude.Should().BeApproximately(-126.453, 0.00001);
        }
    }
}
