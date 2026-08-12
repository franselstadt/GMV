using Xunit;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using FluentAssertions;
using gmvTestConstants;

namespace gmvTM.Server.IntegrationTests
{
    [Collection(nameof(ApiIntegrationCollection))]
    public sealed class WhenQueryingAndReseeding : WithGmvTMApi
    {
        public WhenQueryingAndReseeding(GmvTMWebApplicationFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task ItShouldReturnStopItemsThroughOData()
        {
            HttpResponseMessage response = await Subject.GetAsync(gmvTest.Api.ODataStopItems);
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            using JsonDocument document = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
            document.RootElement.GetProperty("value").GetArrayLength().Should().BeGreaterThan(0);
        }

        [Fact]
        public async Task ItShouldFilterRouteItemsThroughOData()
        {
            HttpResponseMessage response = await Subject.GetAsync(gmvTest.Api.ODataRouteItemsFilteredToF);
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            using JsonDocument document = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
            JsonElement value = document.RootElement.GetProperty("value");
            value.GetArrayLength().Should().Be(1);
            value[0].GetProperty("ShortName").GetString().Should().Be(gmvTest.Data.RouteCode);
        }

        [Fact]
        public async Task ItShouldClearAndReseedTheDatabase()
        {
            HttpResponseMessage reseed = await Subject.PostAsync(gmvTest.Api.DatabaseReseed, null);
            reseed.StatusCode.Should().Be(HttpStatusCode.NoContent);

            HttpResponseMessage stops = await Subject.GetAsync(gmvTest.Api.StopsForRouteF);
            stops.StatusCode.Should().Be(HttpStatusCode.OK);
        }
    }
}
