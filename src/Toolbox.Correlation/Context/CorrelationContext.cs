using System;
using Microsoft.Extensions.OptionsModel;

namespace Toolbox.Correlation
{
    public class CorrelationContext : ICorrelationContext
    {
        private readonly CorrelationOptions _options;

        public CorrelationContext(IOptions<CorrelationOptions> options)
        {
            if ( options.Value == null ) throw new ArgumentNullException(nameof(CorrelationOptions), $"{nameof(CorrelationOptions)} cannot be null.");

            _options = options.Value;

            HeaderName = _options.HeaderKey;
        }

        public string Id { get; private set; }
        public string SourceId { get; private set; }
        public string SourceName { get; private set; }
        public string InstanceId { get; private set; }
        public string InstanceName { get; private set; }
        public string UserId { get; private set; }
        public string IPAddress { get; private set; }
        public string UserToken { get; private set; }

        public string HeaderName { get; private set; } 

        internal bool TrySetValues(string id, string sourceId, string sourceName, string instanceId, string instanceName, string userid = null, string ipaddress = null, string usertoken = null)
        {
            if ( String.IsNullOrWhiteSpace(Id) )
            {
                Id = id;
                SourceId = sourceId;
                SourceName = sourceName;
                InstanceId = instanceId;
                InstanceName = instanceName;
                UserId = userid;
                IPAddress = ipaddress;
                UserToken = usertoken;
                return true;
            }

            return false;
        }

        public void SetValues(string id, string sourceId, string sourceName, string instanceId, string instanceName, string userid = null, string ipaddress = null, string usertoken = null)
        {
            if ( String.IsNullOrWhiteSpace(this.Id) )
            {
                Id = id;
                SourceId = sourceId;
                SourceName = sourceName;
                InstanceId = instanceId;
                InstanceName = instanceName;
            }

            UserId = userid;
            IPAddress = ipaddress;

            if ( String.IsNullOrWhiteSpace(this.UserToken) )
            {
                UserToken = usertoken;
            }
        }
    }
}
