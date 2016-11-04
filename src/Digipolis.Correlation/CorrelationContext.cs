using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
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

            IdHeaderKey = _options.IdHeaderKey;
            SourceHeaderKey = _options.SourceHeaderKey;

        }

        public string Id { get; private set; }
        public string SourceId { get; private set; }
        public string SourceName { get; private set; }
        public string InstanceId { get; private set; }
        public string InstanceName { get; private set; }
        public string UserId { get; private set; }
        public string IpAddress { get; private set; }
        public string IdHeaderKey { get; private set; }
        public string SourceHeaderKey { get; private set; }

        public bool TrySetValues(string id, string sourceId, string sourceName, string instanceId, string instanceName, string userId = null, string ipAddress = null)
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
                return true;
            }

            return false;
        }
    }
}
