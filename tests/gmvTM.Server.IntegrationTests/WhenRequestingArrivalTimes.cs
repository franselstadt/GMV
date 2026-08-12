using Xunit;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using FluentAssertions;
using gmvTM.Domain;
using gmvTestConstants;

namespace gmvTM.Server.IntegrationTests
{
    [Collection(nameof(ApiIntegrationCollection))]
    public sealed class WhenRequestingArrivalTimes : WithGmvTMApi
    {
        public WhenRequestingArrivalTimes(GmvTMWebApplicationFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task ItShouldRequireSimulationThenReturnOneArrivalTime()
        {
            HttpResponseMessage blocked = await Subject.GetAsync(
                gmvTest.Api.NextArrivalsForStop6138);
            blocked.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            HttpResponseMessage start = await Subject.PostAsJsonAsync(
                gmvTest.Api.SimulationsForRouteF,
                ItemFactory.CreateItem<StartSimulationRequest>(new
                {
                    StopCode = gmvTest.Data.SimulationStartStopCode,
                    AverageMph = gmvTest.Data.AverageMph,
                    AverageDwellSeconds = gmvTest.Data.AverageDwellSeconds
                }));
            start.StatusCode.Should().Be(HttpStatusCode.OK);

            HttpResponseMessage arrivalsResponse = await Subject.GetAsync(
                gmvTest.Api.NextArrivalsForStop6138);
            arrivalsResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            NextArrivalDto? arrival = await arrivalsResponse.Content.ReadFromJsonAsync<NextArrivalDto>();
            arrival.Should().NotBeNull();
            arrival!.StopCode.Should().Be(gmvTest.Data.NextArrivalStopCode);
            arrival.PlannedTime.Should().NotBe(default);
            arrival.ActualTime.Should().NotBeNull();
            arrival.ActualTime!.Value.Should().NotBe(default);
        }
    }
}
