using Digipolis.Errors.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Digipolis.Correlation
{
    public class CorrelationMiddleware
    {
        private ILogger<CorrelationMiddleware> _logger;
        private readonly ICorrelationService _correlationService;
        private readonly IOptions<CorrelationOptions> _options;
        private readonly ICorrelationContextFormatter _correlationContextFormatter;
        private readonly RequestDelegate _next;

        public CorrelationMiddleware(RequestDelegate next,
            ILogger<CorrelationMiddleware> logger,
            ICorrelationService correlationService,
            IOptions<CorrelationOptions> options,
            ICorrelationContextFormatter correlationContextFormatter)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next), $"{nameof(next)} cannot be null.");
            _logger = logger ?? throw new ArgumentNullException(nameof(logger), $"{nameof(logger)} cannot be null.");
            _correlationService = correlationService ?? throw new ArgumentNullException(nameof(correlationService));
            _options = options ?? throw new ArgumentNullException(nameof(options));
            _correlationContextFormatter = correlationContextFormatter ?? throw new ArgumentNullException(nameof(correlationContextFormatter));
        }

        public Task Invoke(HttpContext context)
        {
            var correlationHeader = context.Request?.Headers?.FirstOrDefault(x => x.Key.ToLowerInvariant() == CorrelationHeader.Key.ToLowerInvariant()).Value;
            var correlationHeaderValue = !correlationHeader.HasValue || !correlationHeader.Value.Any() ? String.Empty : correlationHeader.Value.FirstOrDefault();

            if (_options.Value.CorrelationHeaderRequired &&
            !Regex.IsMatch(context.Request.Path, _options.Value.CorrelationHeaderNotRequiredRouteRegex))
            {
                if (StringValues.IsNullOrEmpty(correlationHeaderValue))
                {
                    _logger.LogWarning("CorrelationHeader is required.");
                    var exception = new ValidationException("CorrelationHeader is required.", ErrorCode.RequiredCorrelationHeader);
                    exception.AddMessage("CorrelationHeader", "CorrelationHeader is required.");
                    throw exception;
                }
            }
            if (string.IsNullOrWhiteSpace(correlationHeaderValue))
            {
                correlationHeaderValue = _correlationService.GetContext().DgpHeader;
            }
            _correlationContextFormatter.ValidateAndSetPropertiesFromDgpHeader(correlationHeaderValue);

            return _next.Invoke(context);
        }
    }
}
