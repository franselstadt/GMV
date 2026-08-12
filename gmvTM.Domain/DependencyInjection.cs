using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using gmvTM.Domain.Infrastructure;
using gmvTM.Domain.Workers;
using gmvTM.Domain.Workers.Interfaces;

namespace gmvTM.Domain
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddDomain(this IServiceCollection services, IConfiguration config, ORMType ormType = ORMType.EFCore)
        {
            services.AddInfrastructure(config, ormType);

            services.AddSingleton<IDateTimeProviderWorker, SystemDateTimeProviderWorker>();
            services.AddSingleton<IPolylineDecoderWorker, PolylineDecoderWorker>();
            services.AddSingleton<IRoutePathBuilderWorker, RoutePathBuilderWorker>();
            services.AddSingleton<ITripPositionCalculatorWorker, TripPositionCalculatorWorker>();

            return services;
        }
    }
}
