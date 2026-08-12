using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using gmvTM.Domain.Collections;
using gmvTM.Domain.Collections.Interfaces;
using gmvTM.Domain.Infrastructure.Persistence;
using gmvTM.Domain.Workers;
using gmvTM.Domain.Workers.Interfaces;

namespace gmvTM.Domain.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
        {
            services.AddDbContext<DatabaseContext>(options =>
            {
                string connectionString = config.GetConnectionString(Resources.DefaultConnectionStringName) ?? Resources.DefaultSqliteConnection;
                options.UseSqlite(connectionString);
            });

            services.AddScoped<IDatabaseContext>(sp => sp.GetRequiredService<DatabaseContext>());
            services.AddScoped<IRouteCollection, RouteCollection>();
            services.AddScoped<IStopCollection, StopCollection>();
            services.AddScoped<IVehicleCollection, VehicleCollection>();
            services.AddScoped<ITripCollection, TripCollection>();
            services.AddScoped<IStopPlanCollection, StopPlanCollection>();
            services.AddScoped<IStopTripCollection, StopTripCollection>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IDataSeederWorker, DataSeederWorker>();

            return services;
        }
    }
}
