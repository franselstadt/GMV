using Xunit;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using FluentAssertions;
using gmvTM.Domain;
using gmvTM.Domain.Items;
using gmvTestConstants;

namespace gmvTM.Server.IntegrationTests
{
    [Collection(nameof(ApiIntegrationCollection))]
    public sealed class WhenRequestingStops : WithGmvTMApi
    {
        public WhenRequestingStops(GmvTMWebApplicationFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task ItShouldReturnStopItemsForRouteF()
        {
            HttpResponseMessage response = await Subject.GetAsync(gmvTest.Api.StopsForRouteF);
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            PagedResult<StopItem>? stops = await response.Content.ReadFromJsonAsync<PagedResult<StopItem>>();
            stops.Should().NotBeNull();
            stops!.Items.Should().NotBeEmpty();
            stops.Items.Should().BeInAscendingOrder(s => s.Sequence);
        }

        [Fact]
        public async Task ItShouldReturnNotFoundForUnknownRoute()
        {
            HttpResponseMessage response = await Subject.GetAsync(gmvTest.Api.StopsForUnknownRoute);
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}
