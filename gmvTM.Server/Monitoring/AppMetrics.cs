using Prometheus;

namespace gmvTM.Server.Monitoring
{
    //simple implementation
    public static class AppMetrics
    {
        public static readonly Counter SimulationsStarted = Prometheus.Metrics.CreateCounter(
            "gmvtm_simulations_started_total",
            "Total number of vehicle simulations started.",
            new CounterConfiguration { LabelNames = new[] { "route" } });

        public static readonly Counter SimulationsStopped = Prometheus.Metrics.CreateCounter(
            "gmvtm_simulations_stopped_total",
            "Total number of vehicle simulations stopped via the API.");

        public static readonly Gauge ActiveSimulations = Prometheus.Metrics.CreateGauge(
            "gmvtm_simulations_active",
            "Number of currently active vehicle simulations.");

        public static readonly Counter NextArrivalRequests = Prometheus.Metrics.CreateCounter(
            "gmvtm_next_arrival_requests_total",
            "Total number of next-arrival lookups.",
            new CounterConfiguration { LabelNames = new[] { "route" } });
    }
}
