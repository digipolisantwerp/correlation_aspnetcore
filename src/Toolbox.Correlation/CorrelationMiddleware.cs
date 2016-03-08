using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using System;
using System.Threading.Tasks;

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

        public Task Invoke(HttpContext context)
        {
            string correlationId = String.Empty;
            string correlationSource = String.Empty;

            var correlationContext = context.RequestServices.GetService<ICorrelationContext>() as CorrelationContext;
            var correlationIdHeader = context.Request.Headers[correlationContext.IdHeaderKey];

            if (StringValues.IsNullOrEmpty(correlationIdHeader))
            {
                correlationId =  Guid.NewGuid().ToString();
                correlationSource = _source;
            }
            else
            {
                correlationId = correlationIdHeader;
                correlationSource = context.Request.Headers[correlationContext.SourceHeaderKey];
            }

            correlationContext.TrySetValues(correlationId.ToString(), correlationSource);

            _logger.LogInformation($"CorrelationId: {correlationId.ToString()}");
            _logger.LogInformation($"CorrelationSource: {correlationSource}");

            return _next.Invoke(context);
        }
    }
}
