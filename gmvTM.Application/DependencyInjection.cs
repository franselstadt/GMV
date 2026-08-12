using Microsoft.Extensions.DependencyInjection;
using gmvTM.Application.Classes.Services;
using gmvTM.Application.Classes.Simulation;
using gmvTM.Application.Interfaces;

namespace gmvTM.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddSingleton<ISimulationStore, InMemorySimulationStore>();
            services.AddScoped<IRouteStopService, RouteStopService>();
            services.AddScoped<IRoutesService, RoutesService>();
            services.AddScoped<IVehiclesService, VehiclesService>();
            services.AddScoped<ISimulationService, SimulationService>();
            return services;
        }
    }
}
