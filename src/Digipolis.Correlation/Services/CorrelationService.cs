using Digipolis.ApplicationServices;
using Digipolis.Correlation.Helpers;
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
        private readonly IScopedCorrelationContext _correlationContext;

        public CorrelationService(IHttpContextAccessor httpContextAccessor, IApplicationContext applicationContext, ILogger<CorrelationService> logger, ICorrelationContextFormatter correlationContextFormatter, IScopedCorrelationContext correlationContext)
        {
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _applicationContext = applicationContext ?? throw new ArgumentNullException(nameof(applicationContext));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _correlationContextFormatter = correlationContextFormatter ?? throw new ArgumentNullException(nameof(correlationContextFormatter));
            _correlationContext = correlationContext ?? throw new ArgumentNullException(nameof(correlationContext));
        }

        public CorrelationContext GetContext()
        {
            var request = _httpContextAccessor?.HttpContext?.Request;
            var headerValues = request?.Headers?.FirstOrDefault(x => x.Key.ToLowerInvariant() == CorrelationHeader.Key.ToLowerInvariant()).Value;

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

            if (_correlationContext.Context == null)
            {
                _correlationContext.Context = new CorrelationContext
                {
                    Id = Guid.NewGuid().ToString(),
                    SourceId = _applicationContext.ApplicationId,
                    SourceName = _applicationContext.ApplicationName,
                    InstanceId = _applicationContext.InstanceId,
                    InstanceName = _applicationContext.InstanceName,
                    IpAddress = _httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString() ?? String.Empty
                };
                _correlationContext.Context.SetDgpHeader();

                _logger.LogDebug($"CorrelationHeader not found, created correlationId with header value '{_correlationContext.Context.DgpHeader}'");
            }

            return _correlationContext.Context;

        }
    }
}
