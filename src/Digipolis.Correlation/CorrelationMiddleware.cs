using Digipolis.ApplicationServices;
using Digipolis.Errors.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Digipolis.Correlation
{
    public class CorrelationMiddleware
    {
        private ILogger<CorrelationMiddleware> _logger;
        private readonly RequestDelegate _next;
        private IApplicationContext _applicationContext;

        public CorrelationMiddleware(RequestDelegate next,ILogger<CorrelationMiddleware> logger, IApplicationContext applicationContext)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next), $"{nameof(next)} cannot be null.");
            _logger = logger ?? throw new ArgumentNullException(nameof(logger), $"{nameof(logger)} cannot be null.");
            _applicationContext = applicationContext ?? throw new ArgumentNullException(nameof(applicationContext), $"{nameof(applicationContext)} cannot be null.");
        }

        public Task Invoke(HttpContext context, IOptions<CorrelationOptions> options)
        {
            string correlation = String.Empty;
            var correlationContext = context.RequestServices.GetService(typeof(ICorrelationContext)) as CorrelationContext;
            var correlationHeader = context.Request.Headers[CorrelationHeaders.HeaderKey];

            if (!StringValues.IsNullOrEmpty(correlationHeader))
            {
                FillCorrelationContext(correlationHeader, correlationContext);
            }
            else if (options.Value.CorrelationHeaderRequired && !Regex.IsMatch(context.Request.Path,options.Value.CorrelationHeaderNotRequiredRouteRegex))
            {
                _logger.LogWarning("CorrelationHeader is required.");
                var exception = new ValidationException("CorrelationHeader is required.", "REQCOR");
                exception.AddMessage("CorrelationHeader", "CorrelationHeader is required.");

                throw exception;
            }
            else
            {
                correlationContext.TrySetValues(Guid.NewGuid().ToString(),
                     _applicationContext.ApplicationId,
                     _applicationContext.ApplicationName,
                     _applicationContext.InstanceId,
                     _applicationContext.InstanceName);

                _logger.LogDebug("CorrelationHeader not found, created correlationId.");
            }

            return _next.Invoke(context);
        }

        internal void FillCorrelationContext(string correlationHeader, CorrelationContext context)
        {
            try
            {
                byte[] data = Convert.FromBase64String(correlationHeader);
                string json = Encoding.UTF8.GetString(data);

                dynamic parsedHeader = JObject.Parse(json);
                string correlationId = (string)parsedHeader.id;

                if (String.IsNullOrEmpty(correlationId))
                {
                    _logger.LogWarning("Invalid correlationheader, id is required." + correlationHeader);
                    var exception = new ValidationException("Invalid correlationHeader, id is required.", "REQCOR");
                    exception.AddMessage("Invalid correlationHeader, id is required.");
                    throw exception;
                }

                context.TrySetValues(correlationId,
                    (string)parsedHeader.sourceId,
                    (string)parsedHeader.sourceName,
                    (string)parsedHeader.instanceId,
                    (string)parsedHeader.instanceName,
                    (string)parsedHeader.userId,
                    (string)parsedHeader.ipAddress);
            }
            catch (FormatException)
            {
                _logger.LogWarning("Invalid correlationheader " + correlationHeader);
                var exception = new ValidationException("Invalid correlationHeader.", "REQCOR");
                exception.AddMessage("Invalid correlationHeader, id is required.");
                throw exception;
            }
        }
    }
}
