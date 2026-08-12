using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using gmvTM.Domain.Infrastructure;
using gmvTM.Domain.Workers;
using gmvTM.Domain.Workers.Interfaces;

namespace gmvTM.Domain
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddDomain(this IServiceCollection services, IConfiguration config)
        {
            services.AddInfrastructure(config);

            services.AddSingleton<IDateTimeProviderWorker, SystemDateTimeProviderWorker>();
            services.AddSingleton<IPolylineDecoderWorker, PolylineDecoderWorker>();
            services.AddSingleton<IRoutePathBuilderWorker, RoutePathBuilderWorker>();
            services.AddSingleton<ITripPositionCalculatorWorker, TripPositionCalculatorWorker>();

            services.AddScoped<INextArrivalsCalculatorWorker, NextArrivalsCalculatorWorker>();
            services.AddScoped<ITripPathCalculatorWorker, TripPathCalculatorWorker>();

            return services;
        }
    }
}
