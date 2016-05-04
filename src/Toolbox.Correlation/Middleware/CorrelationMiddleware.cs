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
using Toolbox.Errors;

namespace Toolbox.Correlation
{
    internal class CorrelationIdMiddleware
    {
        private ILogger<CorrelationIdMiddleware> _logger;
        private readonly RequestDelegate _next;
        private readonly string _sourceId;
        private readonly string _sourceName;
        private readonly string _instanceId;
        private readonly string _instanceName;

        public CorrelationIdMiddleware(RequestDelegate next, string sourceId, string sourceName, string instanceId, string instanceName, ILogger<CorrelationIdMiddleware> logger)
        {
            if ( next == null ) throw new ArgumentNullException(nameof(next), $"{nameof(next)} cannot be null.");
            if ( String.IsNullOrWhiteSpace(sourceId) ) throw new ArgumentNullException(nameof(sourceId), $"{nameof(sourceId)} cannot be null or empty.");
            if ( String.IsNullOrWhiteSpace(sourceName) ) throw new ArgumentNullException(nameof(sourceName), $"{nameof(sourceName)} cannot be null or empty.");
            if ( String.IsNullOrWhiteSpace(instanceId) ) throw new ArgumentNullException(nameof(instanceId), $"{nameof(instanceId)} cannot be null or empty.");
            if ( String.IsNullOrWhiteSpace(instanceName) ) throw new ArgumentNullException(nameof(instanceName), $"{nameof(instanceName)} cannot be null or empty.");
            if (logger == null) throw new ArgumentNullException(nameof(logger), $"{nameof(logger)} cannot be null.");

            _next = next;
            _sourceId = sourceId;
            _sourceName = sourceName;
            _instanceId = instanceId;
            _instanceName = instanceName;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context, IOptions<CorrelationOptions> options)
        {
            var correlationId = String.Empty;
            var correlationSourceId = String.Empty;
            var correlationSourceName = String.Empty;
            var correlationInstanceId = String.Empty;
            var correlationInstanceName = String.Empty;
            var userid = String.Empty;
            var ipaddress = String.Empty;
            var usertoken = String.Empty; 

            var correlationContext = context.RequestServices.GetService<ICorrelationContext>() as CorrelationContext;
            var correlationHeader = context.Request.Headers[options.Value.HeaderKey];

            if ( StringValues.IsNullOrEmpty(correlationHeader))
            {
                correlationId =  Guid.NewGuid().ToString();
                correlationSourceId = _sourceId;
                correlationSourceName = _sourceName;
                correlationInstanceId = _instanceId;
                correlationInstanceName = _instanceName;
            }
            else
            {
                try
                {
                    var bytes = Convert.FromBase64String(correlationHeader);
                    var headerString = Encoding.UTF8.GetString(bytes);
                    var header = JsonConvert.DeserializeObject<CorrelationHeader>(headerString);

                    // ToDo (SVB) : check if header was successfully deserialized

                    // ==> FormatException opvangen en custom exception ?

                    // Tests : not base64, not correlationheader object, missing properties, extra property

                    correlationId = header.Id ?? String.Empty;
                    correlationSourceId = header.SourceId ?? String.Empty;
                    correlationSourceName = header.SourceName ?? String.Empty;
                    correlationInstanceId = header.InstanceId ?? String.Empty;
                    correlationInstanceName = header.InstanceName ?? String.Empty;
                    userid = header.UserId ?? String.Empty;
                    ipaddress = header.IPAddress ?? String.Empty;
                    usertoken = header.UserToken ?? String.Empty;
                }
                catch ( FormatException ex )
                {
                    context.Response.StatusCode = 400;
                    var error = new Error();
                    error.AddMessage("CorrelationHeader", $"Invalid Correlation header ({ex.Message})");   // ToDo (SVB) : testen
                    var jsonString = JsonConvert.SerializeObject(error);
                    await context.Response.WriteAsync(jsonString);
                    return;
                }
            }

            correlationContext.TrySetValues(correlationId, correlationSourceId, correlationSourceName, correlationInstanceId, correlationInstanceName, userid, ipaddress, usertoken);

            _logger.LogDebug($"CorrelationId: {correlationId}");
            _logger.LogDebug($"CorrelationSourceName: {correlationSourceName}");
            _logger.LogDebug($"CorrelationInstanceName: {correlationInstanceName}");
            _logger.LogDebug($"CorrelationUser: {userid}");

            await _next.Invoke(context);
        }
    }
}
