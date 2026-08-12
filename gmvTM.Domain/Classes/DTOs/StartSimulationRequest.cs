using System.Text.Json.Serialization;

using gmvTM.Domain.Classes.DTOs.Base;

namespace gmvTM.Domain
{
    public class StartSimulationRequest : BaseDTO
    {
        public string StopCode { get; set; }
        public double? AverageMph { get; set; }
        public int? AverageDwellSeconds { get; set; }

        [JsonPropertyName("vehicleId")]
        public int? VehicleID { get; set; }
    }
}
