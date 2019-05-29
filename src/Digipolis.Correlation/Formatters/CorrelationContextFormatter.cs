using Digipolis.Errors.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Text;

namespace Digipolis.Correlation
{
    public class CorrelationContextFormatter : ICorrelationContextFormatter
    {
        private readonly ILogger<CorrelationContextFormatter> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CorrelationContextFormatter(ILogger<CorrelationContextFormatter> logger, IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }
        public CorrelationContext ValidateAndSetPropertiesFromDgpHeader(string header)
        {
            try
            {
                var context = JsonConvert.DeserializeObject<CorrelationContext>(Encoding.UTF8.GetString(Convert.FromBase64String(header)));
                if (String.IsNullOrEmpty(context.Id))
                {
                    ReplaceInvalidContextByDefault();
                    _logger.LogWarning($"Invalid correlationheader, id is required. Header value: '{header}'");
                    var exception = new ValidationException("Invalid correlationheader, id is required.", ErrorCode.InvalidCorrelationHeader);
                    exception.AddMessage($"Invalid correlationheader, id is required. Header value: '{header}'");
                    throw exception;
                }
                context.DgpHeader = header;
                return context;
            }
            catch (Exception)
            {
                ReplaceInvalidContextByDefault();
                _logger.LogError($"Invalid correlationheader '{header}'");
                var exception = new ValidationException("Invalid correlationheader.", ErrorCode.InvalidCorrelationHeader);
                exception.AddMessage($"Invalid correlationheader '{header}'");
                throw exception;
            }
        }

        /*
         * The logger might contain a correlation enricher extension that also calls the correlationcontextformatter.
         * In order to enable the logging of invalid correlation headers without causing an infinite loop,
         * the original correlation header is being overwritten with a default header
         */
        private void ReplaceInvalidContextByDefault()
        {
            var correlationHeader = _httpContextAccessor.HttpContext.Request?.Headers?.FirstOrDefault(x => x.Key.ToLowerInvariant() == CorrelationHeader.Key.ToLowerInvariant());
            if (!String.IsNullOrWhiteSpace(correlationHeader?.Key))
                _httpContextAccessor.HttpContext.Request.Headers.Remove(correlationHeader.Value.Key);

            _httpContextAccessor.HttpContext.Request.Headers[CorrelationHeader.Key] = CorrelationHeader.Default;
        }
    }
}
