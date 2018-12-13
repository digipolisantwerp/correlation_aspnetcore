using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Text;

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

        [JsonProperty("dgpHeader")]
        [JsonIgnore]
        public string DgpHeader { get; private set; }

        public bool TrySetValues(string id, string sourceId, string sourceName, string instanceId, string instanceName, string userId = null, string ipAddress = null, string dgpHeader = null)
        {
            if (string.IsNullOrWhiteSpace(Id))
            {
                Id = id;
                SourceId = sourceId;
                SourceName = sourceName;
                InstanceId = instanceId;
                InstanceName = instanceName;
                UserId = userId;
                IpAddress = ipAddress;

                if (dgpHeader == null)
                {
                    StringBuilder sb = new StringBuilder();
                    StringWriter sw = new StringWriter(sb);
                    using (JsonWriter writer = new JsonTextWriter(sw))
                    {
                        writer.Formatting = Formatting.Indented;
                        writer.WriteStartObject();
                        writer.WritePropertyName("id");
                        writer.WriteValue(Id);
                        writer.WritePropertyName("sourceId");
                        writer.WriteValue(SourceId);
                        writer.WritePropertyName("sourceName");
                        writer.WriteValue(SourceName);
                        writer.WritePropertyName("instanceId");
                        writer.WriteValue(InstanceId);
                        writer.WritePropertyName("instanceName");
                        writer.WriteValue(InstanceName);
                        writer.WritePropertyName("userId");
                        writer.WriteValue(UserId);
                        writer.WritePropertyName("ipAddress");
                        writer.WriteValue(IpAddress);
                    }
                    var json = sb.ToString();
                    var jsonAsBytes = Encoding.UTF8.GetBytes(json);
                    dgpHeader = Convert.ToBase64String(jsonAsBytes);
                }
                DgpHeader = dgpHeader;
                return true;
            }

            return false;
        }
    }
}
