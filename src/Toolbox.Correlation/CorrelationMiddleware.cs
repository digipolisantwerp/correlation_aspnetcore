using System;
using System.Threading.Tasks;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.OptionsModel;
using Microsoft.Extensions.Primitives;

namespace Toolbox.Correlation
{
    internal class CorrelationIdMiddleware
    {
        private ILogger<CorrelationIdMiddleware> _logger;
        private readonly RequestDelegate _next;
        private string _source;

        public CorrelationIdMiddleware(RequestDelegate next, string source, ILogger<CorrelationIdMiddleware> logger)
        {
            if (next == null) throw new ArgumentNullException(nameof(next), $"{nameof(next)} cannot be null.");
            if (String.IsNullOrWhiteSpace(source)) throw new ArgumentNullException(nameof(source), $"{nameof(source)} cannot be null or empty.");
            if (logger == null) throw new ArgumentNullException(nameof(logger), $"{nameof(logger)} cannot be null.");

            _next = next;
            _source = source;
            _logger = logger;
        }

        public Task Invoke(HttpContext context, IOptions<CorrelationOptions> options)
        {
            var correlationId = String.Empty;
            var correlationSource = String.Empty;
            var userid = String.Empty;
            var ipaddress = String.Empty;
            var usertoken = String.Empty; 

            var correlationContext = context.RequestServices.GetService<ICorrelationContext>() as CorrelationContext;
            var correlationHeader = context.Request.Headers[options.Value.HeaderKey];

            if (StringValues.IsNullOrEmpty(correlationHeader))
            {
                correlationId =  Guid.NewGuid().ToString();
                correlationSource = _source;
            }
            else
            {
                correlationId = correlationHeader;
            }

            correlationContext.TrySetValues(correlationId.ToString(), correlationSource, userid, ipaddress, usertoken);

            _logger.LogInformation($"CorrelationId: {correlationId.ToString()}");
            _logger.LogInformation($"CorrelationSource: {correlationSource}");

            return _next.Invoke(context);
        }
    }
}
