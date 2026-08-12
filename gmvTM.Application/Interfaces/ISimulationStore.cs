using System.Collections.Generic;
using gmvTM.Application.Classes.Simulation;

namespace gmvTM.Application.Interfaces
{
    public interface ISimulationStore
    {
        public void Add(ActiveSimulation simulation);

        public bool TryGet(int id, out ActiveSimulation? simulation);       
        public IReadOnlyList<ActiveSimulation> ListActive();

        public bool TryRemove(int id, out ActiveSimulation? simulation);
    }
}
