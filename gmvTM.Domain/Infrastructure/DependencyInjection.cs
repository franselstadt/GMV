using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using gmvTM.Domain.Collections.Interfaces;
using gmvTM.Domain.Workers.Interfaces;
using gmvTM.Domain.Infrastructure.Interfaces;

namespace gmvTM.Domain.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config, ORMType ormType = ORMType.EFCore)
        {
            switch (ormType)
            {
                case ORMType.EFCore:
                    return services.AddEFCore(config);
                default:
                    throw new NotSupportedException(string.Format(gmvDomain.Messages.OrmTypeNotSupported, ormType));
            }
        }

        private static IServiceCollection AddEFCore(this IServiceCollection services, IConfiguration config)
        {
            services.AddDbContext<Strategies.ORM.EFCore.Infrastructure.DatabaseContext>(options =>
            {
                string connectionString = config.GetConnectionString(gmvDomain.Resources.DefaultConnectionStringName) ?? gmvDomain.Resources.DefaultSqliteConnection;
                options.UseSqlite(connectionString);
            });

            services.AddScoped<IDatabaseContext>(sp => sp.GetRequiredService<Strategies.ORM.EFCore.Infrastructure.DatabaseContext>());
            services.AddScoped<IRouteCollection, Strategies.ORM.EFCore.Collections.RouteCollection > ();
            services.AddScoped<IStopCollection, Strategies.ORM.EFCore.Collections.StopCollection>();
            services.AddScoped<IVehicleCollection, Strategies.ORM.EFCore.Collections.VehicleCollection>();
            services.AddScoped<ITripCollection, Strategies.ORM.EFCore.Collections.TripCollection>();
            services.AddScoped<IStopPlanCollection, Strategies.ORM.EFCore.Collections.StopPlanCollection>();
            services.AddScoped<IStopTripCollection, Strategies.ORM.EFCore.Collections.StopTripCollection>();
            services.AddScoped<ISimpleUnitOfWork, Strategies.ORM.EFCore.Infrastructure.UnitOfWork>();
            services.AddScoped<IDataSeederWorker, Strategies.ORM.EFCore.Workers.DataSeederWorker>();
            services.AddScoped<ITripPathCalculatorWorker, Strategies.ORM.EFCore.Workers.TripPathCalculatorWorker>();

            return services;
        }
    }
}
