using Newtonsoft.Json;

namespace Digipolis.Correlation
{
    public class CorrelationContext
    {
        public CorrelationContext()
        {
        }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("sourceId")]
        public string SourceId { get; set; }

        [JsonProperty("sourceName")]
        public string SourceName { get; set; }

        [JsonProperty("instanceId")]
        public string InstanceId { get; set; }

        [JsonProperty("instanceName")]
        public string InstanceName { get; set; }

        [JsonProperty("userId")]
        public string UserId { get; set; }

        [JsonProperty("ipAddress")]
        public string IpAddress { get; set; }

        [JsonProperty("dgpHeader")]
        [JsonIgnore]
        public string DgpHeader { get; set; }
    }
}
