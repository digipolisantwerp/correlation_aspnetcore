using Digipolis.ApplicationServices;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;

namespace Digipolis.Correlation
{
    public class CorrelationService : ICorrelationService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IApplicationContext _applicationContext;
        private readonly ILogger<CorrelationService> _logger;
        private readonly ICorrelationContextFormatter _correlationContextFormatter;
        public CorrelationService(IHttpContextAccessor httpContextAccessor, IApplicationContext applicationContext, ILogger<CorrelationService> logger, ICorrelationContextFormatter correlationContextFormatter)
        {
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _applicationContext = applicationContext ?? throw new ArgumentNullException(nameof(applicationContext));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _correlationContextFormatter = correlationContextFormatter ?? throw new ArgumentNullException(nameof(correlationContextFormatter));
        }

        public CorrelationContext GetContext()
        {
            var request = _httpContextAccessor?.HttpContext?.Request;
            var headerValues = request?.Headers?.FirstOrDefault(x => x.Key == CorrelationHeader.Key).Value;

            if (request == null)
                _logger.LogDebug($"{GetType().Name}.CorrelationContext - No incoming request");
            if (!headerValues.HasValue || !headerValues.Value.Any())
                _logger.LogDebug($"{GetType().Name}.CorrelationContext - No correlation header found in incoming request");
            else if (headerValues.Value.Count > 1)
                _logger.LogError($"{GetType().Name}.CorrelationContext( - Multiple correlation headers found in incoming request");
            else
            {
                string correlationHeader = headerValues.Value.FirstOrDefault();
                var context = _correlationContextFormatter.ValidateAndSetPropertiesFromDgpHeader(correlationHeader);
                return context;
            }

            var correlationContext = new CorrelationContext
            {
                Id = Guid.NewGuid().ToString(),
                SourceId = _applicationContext.ApplicationId,
                SourceName = _applicationContext.ApplicationName,
                InstanceId = _applicationContext.InstanceId,
                InstanceName = _applicationContext.InstanceName,
                IpAddress = _httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString() ?? String.Empty
            };
            correlationContext.SetDgpHeader();

            _logger.LogDebug($"CorrelationHeader not found, created correlationId with header value '{correlationContext.DgpHeader}'");

            return correlationContext;

        }
    }
}
