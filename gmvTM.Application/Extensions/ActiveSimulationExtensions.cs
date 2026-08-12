using gmvTM.Application.Classes.Simulation;
using gmvTM.Domain;

namespace gmvTM.Application.Extensions
{
    public static class ActiveSimulationExtensions
    {
        public static SimulationRunDto ToDto(this ActiveSimulation simulation)
        {
            return ItemFactory.CreateItem<SimulationRunDto>(new
            {
                simulation.ID,
                simulation.RouteCode,
                simulation.VehicleID,
                simulation.VehicleNumber,
                simulation.TripID,
                simulation.Status,
                simulation.StartStopCode,
                simulation.AverageMph,
                simulation.AverageDwellSeconds,
                simulation.StartedAtUtc
            });
        }
    }
}
