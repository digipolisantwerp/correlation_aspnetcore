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

            HeaderKey = _options.HeaderKey;
        }

        public string Id { get; private set; }
        public string Source { get; private set; }
        public string UserId { get; private set; }
        public string IPAddress { get; private set; }
        public string UserToken { get; private set; }

        public string HeaderKey { get; private set; } 

        internal bool TrySetValues(string id, string source, string userid = null, string ipaddress = null, string usertoken = null)
        {
            if (String.IsNullOrWhiteSpace(Id))
            {
                Id = id;
                Source = source;
                UserId = userid;
                IPAddress = ipaddress;
                UserToken = usertoken;
                return true;
            }

            return false;
        }
    }
}
