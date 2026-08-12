using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Asp.Versioning;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.OData;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;
using Microsoft.OpenApi.Models;
using Prometheus;
using Serilog;
using gmvTM.Application;
using gmvTM.Application.Interfaces;
using gmvTM.Domain;
using gmvTM.Domain.Items;
using gmvTM.Domain.Workers.Interfaces;
using gmvTM.Server.Classes.Tools;
using gmvTM.Server.Hubs;
using gmvTM.Server.Middleware;
using gmvTM.Server.Realtime;

namespace gmvTM.Server
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

            builder.Host.UseSerilog((context, services, loggerConfiguration) => loggerConfiguration
                .ReadFrom.Configuration(context.Configuration)
                .ReadFrom.Services(services)
                .Enrich.FromLogContext());

            builder.Services.AddApplication();
            builder.Services.AddDomain(builder.Configuration);
            builder.Services.AddSingleton<IVehiclePositionBroadcaster, SignalRVehiclePositionBroadcaster>();
            builder.Services.AddHostedService<TripSimulationBackgroundService>();
            builder.Services.AddSignalR();
            builder.Services.AddControllers().AddOData(options => options
                .Select()
                .Filter()
                .OrderBy()
                .Count()
                .Expand()
                .SetMaxTop(200)
                .AddRouteComponents("odata", BuildEdmModel()));

            builder.Services.AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ReportApiVersions = true;
                options.ApiVersionReader = new UrlSegmentApiVersionReader();
            })
            .AddMvc()
            .AddApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
            });

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = gmvDomain.Messages.AppTitle,
                    Version = "v1",
                    Description = gmvServer.Messages.SwaggerDescription
                });

                options.AddSecurityDefinition(gmvServer.Security.BasicSchemeName, new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = gmvServer.Security.BasicSchemeName,
                    In = ParameterLocation.Header,
                    Description = gmvServer.Messages.SwaggerBasicAuthDescription
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = gmvServer.Security.BasicSchemeName
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });

            builder.Services.AddCors(options =>
            {
                options.AddPolicy(gmvDomain.AppConstants.CorsPolicyName, policy =>
                {
                    policy.SetIsOriginAllowed(_ => true).AllowAnyHeader().AllowAnyMethod().AllowCredentials();
                });
            });

            WebApplication app = builder.Build();

            ItemFactory.Logger = app.Services.GetRequiredService<ILoggerFactory>().CreateLogger(nameof(ItemFactory));

            app.Logger.LogInformation(gmvServer.Messages.LogStartingApi, app.Environment.EnvironmentName);

            using (IServiceScope scope = app.Services.CreateScope())
            {
                IDataSeederWorker seeder = scope.ServiceProvider.GetRequiredService<IDataSeederWorker>();
                await seeder.SeedAsync().ConfigureAwait(false);
            }

            app.UseSerilogRequestLogging();
            app.UseMiddleware<ExceptionHandlingMiddleware>();
            app.UseHttpsRedirection();
            app.UseCors(gmvDomain.AppConstants.CorsPolicyName);
            app.UseMiddleware<BasicAuthMiddleware>();
            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseHttpMetrics();
            app.MapControllers();
            app.MapMetrics();
            app.MapHub<VehiclePositionHub>(gmvDomain.AppConstants.VehiclePositionHubPath);

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint("/swagger/v1/swagger.json", gmvServer.Messages.SwaggerUiDisplayName);
                });
            }

            app.MapFallbackToFile("/index.html");

            if (app.Environment.IsDevelopment() || Debugger.IsAttached)
            {
                app.Lifetime.ApplicationStarted.Register(() =>
                    DeveloperConsoleTools.Open(app.Urls, app.Configuration[gmvServer.Security.AuthUsernameSetting]));
            }

            app.Logger.LogInformation(gmvServer.Messages.LogApiConfigured);
            await app.RunAsync().ConfigureAwait(false);
        }

        private static IEdmModel BuildEdmModel()
        {
            ODataConventionModelBuilder modelBuilder = new ODataConventionModelBuilder();
            modelBuilder.EntitySet<RouteItem>("RouteItems");
            modelBuilder.EntitySet<StopItem>("StopItems");
            modelBuilder.EntitySet<VehicleItem>("VehicleItems");
            modelBuilder.EntitySet<TripItem>("TripItems");
            modelBuilder.EntitySet<StopPlanItem>("StopPlanItems");
            modelBuilder.EntitySet<StopTripItem>("StopTripItems");
            return modelBuilder.GetEdmModel();
        }
    }
}
