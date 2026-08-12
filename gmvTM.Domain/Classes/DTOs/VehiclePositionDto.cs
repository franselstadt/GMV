using System;
using System.Text.Json.Serialization;

using gmvTM.Domain.Classes.DTOs.Base;

namespace gmvTM.Domain
{
    public class VehiclePositionDto : BaseDTO
    {
        [JsonPropertyName("simulationRunId")]
        public int SimulationRunID { get; set; }

        [JsonPropertyName("tripId")]
        public int TripID { get; set; }

        public string RouteCode { get; set; }

        [JsonPropertyName("vehicleId")]
        public int VehicleID { get; set; }

        public string VehicleNumber { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Phase { get; set; }
        public string StopCode { get; set; }
        public string StopName { get; set; }
        public double? SecondsToStop { get; set; }
        public bool BehindSchedule { get; set; }
        public string ScheduleAlert { get; set; }
        public DateTime AsOfUtc { get; set; }
    }
}
