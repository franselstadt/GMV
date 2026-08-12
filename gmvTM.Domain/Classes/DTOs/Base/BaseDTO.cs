using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace gmvTM.Domain.Classes.DTOs.Base
{
    public abstract class BaseDTO
    {
        [JsonExtensionData]
        public Dictionary<string, object?> DynamicProperties { get; set; } = new Dictionary<string, object?>();
    }
}
