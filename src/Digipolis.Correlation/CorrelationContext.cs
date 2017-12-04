using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;

namespace Digipolis.Correlation
{
    public class CorrelationContext : ICorrelationContext
    {
        private readonly CorrelationOptions _options;

        public CorrelationContext(IOptions<CorrelationOptions> options)
        {
            if (options.Value == null) throw new ArgumentNullException(nameof(CorrelationOptions), $"{nameof(CorrelationOptions)} cannot be null.");

            _options = options.Value;
        }

        [JsonProperty("id")]
        public string Id { get; private set; }

        [JsonProperty("sourceId")]
        public string SourceId { get; private set; }

        [JsonProperty("sourceName")]
        public string SourceName { get; private set; }

        [JsonProperty("instanceId")]
        public string InstanceId { get; private set; }

        [JsonProperty("instanceName")]
        public string InstanceName { get; private set; }

        [JsonProperty("userId")]
        public string UserId { get; private set; }

        [JsonProperty("ipAddress")]
        public string IpAddress { get; private set; }

        [JsonProperty("jwtToken")]
        [JsonIgnore]
        public string JwtToken { get; private set; }

        public bool TrySetValues(string id, string sourceId, string sourceName, string instanceId, string instanceName, string userId = null, string ipAddress = null, string jwtToken = null)
        {
            if (String.IsNullOrWhiteSpace(Id))
            {
                Id = id;
                SourceId = sourceId;
                SourceName = sourceName;
                InstanceId = instanceId;
                InstanceName = instanceName;
                UserId = userId;
                IpAddress = ipAddress;
                if (jwtToken == null)
                {
                    var json = JsonConvert.SerializeObject(this);
                    var jsonAsBytes = System.Text.Encoding.UTF8.GetBytes(json);
                    jwtToken = Convert.ToBase64String(jsonAsBytes);
                }
                JwtToken = jwtToken;
                return true;
            }

            return false;
        }
    }
}
