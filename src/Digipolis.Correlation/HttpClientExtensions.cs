using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;

namespace Digipolis.Correlation
{
    public static class HttpClientExtensions
    {
        public static void SetCorrelationValues(this HttpClient client, ICorrelationContext context)
        {
            if (context == null) throw new NullReferenceException($"{nameof(context)} cannot be null.");

            client.DefaultRequestHeaders.Remove(CorrelationHeaders.HeaderKey);
            client.DefaultRequestHeaders.Add(CorrelationHeaders.HeaderKey, context.DgpHeader);
        }

        public static void SetCorrelationValues(this HttpClient client, IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetService<ICorrelationContext>();

            SetCorrelationValues(client, context);
        }
    }
}
