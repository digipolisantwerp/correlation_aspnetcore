using System;
using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Toolbox.Correlation
{
    public static class HttpClientExtensions
    {
        public static void SetCorrelationValues(this HttpClient client, ICorrelationContext context)
        {
            if (context == null) throw new NullReferenceException($"{nameof(context)} cannot be null.");

            client.DefaultRequestHeaders.Add(context.HeaderName, context.Id.ToString());
        }

        public static void SetCorrelationValues(this HttpClient client, IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetService<ICorrelationContext>();

            SetCorrelationValues(client, context);
        }
    }
}
