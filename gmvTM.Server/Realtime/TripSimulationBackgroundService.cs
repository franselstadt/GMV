using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using gmvTM.Application.Interfaces;
using gmvTM.Domain;
using gmvTM.Server.Monitoring;

namespace gmvTM.Server.Realtime
{
    // Claude and Google assisted me in writing the websocket handling.
    public sealed class TripSimulationBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<TripSimulationBackgroundService> _logger;

        public TripSimulationBackgroundService(IServiceScopeFactory scopeFactory, ILogger<TripSimulationBackgroundService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using PeriodicTimer timer = new PeriodicTimer(TimeSpan.FromMilliseconds(gmvDomain.AppConstants.SimulationTickMilliseconds));

            while (await timer.WaitForNextTickAsync(stoppingToken))
            {
                try
                {
                    using IServiceScope scope = _scopeFactory.CreateScope();
                    ISimulationService simulations = scope.ServiceProvider.GetRequiredService<ISimulationService>();
                    await simulations.TickAsync(stoppingToken);

                    IReadOnlyList<SimulationRunDto> active = await simulations.GetActiveAsync(stoppingToken);
                    AppMetrics.ActiveSimulations.Set(active.Count);
                }
                catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                {
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, gmvServer.Messages.LogSimulationTickFailed);
                }
            }
        }
    }
}
