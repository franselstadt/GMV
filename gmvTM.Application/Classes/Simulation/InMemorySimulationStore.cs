using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using gmvTM.Application.Interfaces;
using gmvTM.Domain;

namespace gmvTM.Application.Classes.Simulation
{
    public sealed class InMemorySimulationStore : ISimulationStore
    {
        //just note in a real production system I would of used redis, this is just a simple in memory store for the purpose of this exercise
        private readonly ConcurrentDictionary<int, ActiveSimulation> _runs = new();

        public void Add(ActiveSimulation simulation) =>
            _runs[simulation.ID] = simulation;

        public bool TryGet(int id, out ActiveSimulation? simulation) =>
            _runs.TryGetValue(id, out simulation);

        public IReadOnlyList<ActiveSimulation> ListActive() =>
            _runs.Values
                .Where(s => string.Equals(s.Status, gmvDomain.Messages.SimulationStatusRunning, StringComparison.Ordinal))
                .ToList();

        public bool TryRemove(int id, out ActiveSimulation? simulation) =>
            _runs.TryRemove(id, out simulation);
    }
}
