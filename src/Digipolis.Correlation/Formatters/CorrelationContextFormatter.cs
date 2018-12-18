using Digipolis.Errors.Exceptions;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Text;

namespace Digipolis.Correlation
{
    public class CorrelationContextFormatter : ICorrelationContextFormatter
    {
        private readonly ILogger<CorrelationContextFormatter> _logger;

        public CorrelationContextFormatter(ILogger<CorrelationContextFormatter> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        public CorrelationContext ValidateAndSetPropertiesFromDgpHeader(string header)
        {
            try
            {
                var context = JsonConvert.DeserializeObject<CorrelationContext>(Encoding.UTF8.GetString(Convert.FromBase64String(header)));

                if (String.IsNullOrEmpty(context.Id))
                {
                    _logger.LogWarning($"Invalid correlationheader, id is required. Header value: '{header}'");
                    var exception = new ValidationException("Invalid correlationheader, id is required.", ErrorCode.InvalidCorrelationHeader);
                    exception.AddMessage($"Invalid correlationheader, id is required. Header value: '{header}'");
                    throw exception;
                }
                context.DgpHeader = header;
                return context;
            }
            catch (FormatException)
            {
                _logger.LogError($"Invalid correlationheader '{header}'");
                var exception = new ValidationException("Invalid correlationheader.", ErrorCode.InvalidCorrelationHeader);
                exception.AddMessage($"Invalid correlationheader '{header}'");
                throw exception;
            }
        }
    }
}
