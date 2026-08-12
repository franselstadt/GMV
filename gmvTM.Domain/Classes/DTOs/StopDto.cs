using System.Text.Json.Serialization;

namespace gmvTM.Domain
{
    public class StopDto
    {
        [JsonPropertyName("id")]
        public int ID { get; set; }

        public string StopCode { get; set; }
        public string Name { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public int Sequence { get; set; }
    }
}
