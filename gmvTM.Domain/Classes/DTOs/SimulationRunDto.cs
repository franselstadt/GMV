using System;
using System.Text.Json.Serialization;

namespace gmvTM.Domain
{
    public class SimulationRunDto
    {
        [JsonPropertyName("id")]
        public int ID { get; set; }

        public string RouteCode { get; set; }

        [JsonPropertyName("vehicleId")]
        public int VehicleID { get; set; }
        public string VehicleNumber { get; set; }

        [JsonPropertyName("tripId")]
        public int TripID { get; set; }

        public string Status { get; set; }
        public string StartStopCode { get; set; }
        public double AverageMph { get; set; }
        public int AverageDwellSeconds { get; set; }

        public DateTime StartedAtUtc { get; set; }
    }
}
