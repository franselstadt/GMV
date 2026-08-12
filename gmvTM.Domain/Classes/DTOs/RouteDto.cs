using System.Text.Json.Serialization;

namespace gmvTM.Domain
{
    public class RouteDto
    {
        [JsonPropertyName("id")]
        public int ID { get; set; }
        public string ShortName { get; set; }
        public string LongName { get; set; }
        public string Color { get; set; }
    }
}
