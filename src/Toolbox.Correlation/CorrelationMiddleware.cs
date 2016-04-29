using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.OptionsModel;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;

namespace Toolbox.Correlation
{
    internal class CorrelationIdMiddleware
    {
        private ILogger<CorrelationIdMiddleware> _logger;
        private readonly RequestDelegate _next;
        private string _source;
        private string _instance;

        public CorrelationIdMiddleware(RequestDelegate next, string source, string instance, ILogger<CorrelationIdMiddleware> logger)
        {
            if (next == null) throw new ArgumentNullException(nameof(next), $"{nameof(next)} cannot be null.");
            if (String.IsNullOrWhiteSpace(source)) throw new ArgumentNullException(nameof(source), $"{nameof(source)} cannot be null or empty.");
            if ( String.IsNullOrWhiteSpace(instance) ) throw new ArgumentNullException(nameof(instance), $"{nameof(instance)} cannot be null or empty.");
            if (logger == null) throw new ArgumentNullException(nameof(logger), $"{nameof(logger)} cannot be null.");

            _next = next;
            _source = source;
            _instance = instance;
            _logger = logger;
        }

        public Task Invoke(HttpContext context, IOptions<CorrelationOptions> options)
        {
            var correlationId = String.Empty;
            var correlationSource = String.Empty;
            var correlationInstance = String.Empty;
            var userid = String.Empty;
            var ipaddress = String.Empty;
            var usertoken = String.Empty; 

            var correlationContext = context.RequestServices.GetService<ICorrelationContext>() as CorrelationContext;
            var correlationHeader = context.Request.Headers[options.Value.HeaderKey];

            if ( StringValues.IsNullOrEmpty(correlationHeader))
            {
                correlationId =  Guid.NewGuid().ToString();
                correlationSource = _source;
                correlationInstance = _instance;
            }
            else
            {
                var bytes = Convert.FromBase64String(correlationHeader);
                var headerString = Encoding.UTF8.GetString(bytes);
                var header = JsonConvert.DeserializeObject<CorrelationHeader>(headerString);

                // ToDo (SVB) : check if header was successfully deserialized

                // Tests : not base64, not correlationheader object, missing properties, extra property

                correlationId = header.Id ?? String.Empty;
                correlationSource = header.Source ?? String.Empty;
                correlationInstance = header.Instance ?? String.Empty;
                userid = header.UserId ?? String.Empty;
                ipaddress = header.IPAddress ?? String.Empty;
                usertoken = header.UserToken ?? String.Empty;
            }

            correlationContext.TrySetValues(correlationId, correlationSource, correlationInstance, userid, ipaddress, usertoken);

            _logger.LogDebug($"CorrelationId: {correlationId}");
            _logger.LogDebug($"CorrelationSource: {correlationSource}");
            _logger.LogDebug($"CorrelationInstance: {correlationInstance}");
            _logger.LogDebug($"CorrelationUser: {userid}");

            return _next.Invoke(context);
        }
    }
}
