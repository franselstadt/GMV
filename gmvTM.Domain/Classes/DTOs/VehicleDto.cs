using System.Text.Json.Serialization;

namespace gmvTM.Domain
{
    public class VehicleDto
    {
        [JsonPropertyName("id")]
        public int ID { get; set; }

        public string FleetCode { get; set; } = null!;
        public string Make { get; set; } = null!;
        public string Model { get; set; } = null!;
        public string LicensePlate { get; set; } = null!;
        public int Capacity { get; set; }
        public int ModelYear { get; set; }
        public bool WheelchairAccessible { get; set; }
    }
}
