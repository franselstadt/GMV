using System.Text.Json.Serialization;

namespace gmvTM.Domain
{
    public class StartSimulationRequest
    {
        public string StopCode { get; set; }
        public double? AverageMph { get; set; }
        public int? AverageDwellSeconds { get; set; }

        [JsonPropertyName("vehicleId")]
        public int? VehicleID { get; set; }
    }
}
