using Xunit;
using FluentAssertions;
using NetArchTest.Rules;
using gmvTM.Domain.Items;
using gmvTM.Application.Classes.Services;

namespace gmvTM.Domain.Tests
{
    public sealed class WhenCheckingArchitecture
    {
        [Fact]
        public void ItShouldPreventDomainDependingOnApplicationOrServer()
        {
            TestResult result = Types.InAssembly(typeof(TripItem).Assembly)
                .Should()
                .NotHaveDependencyOn("gmvTM.Application")
                .And()
                .NotHaveDependencyOn("gmvTM.Server")
                .GetResult();

            result.IsSuccessful.Should().BeTrue(
                because: result.FailingTypeNames is null
                    ? "Domain must not depend on Application or Server"
                    : string.Join(", ", result.FailingTypeNames));
        }

        [Fact]
        public void ItShouldPreventApplicationDependingOnServer()
        {
            TestResult result = Types.InAssembly(typeof(RouteStopService).Assembly)
                .Should()
                .NotHaveDependencyOn("gmvTM.Server")
                .GetResult();

            result.IsSuccessful.Should().BeTrue(
                because: result.FailingTypeNames is null
                    ? "Application must not depend on Server"
                    : string.Join(", ", result.FailingTypeNames));
        }
    }
}
