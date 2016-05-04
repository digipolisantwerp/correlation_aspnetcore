using System.Net.Http;
using Toolbox.Correlation;

namespace SampleApi1
{
    public static class HttpRequestExtensions
    {
        public static void SetCorrelationValues(this HttpRequestMessage request, ICorrelationContext context)
        {
            request.Headers.Add(context.HeaderName, context.Id.ToString());
        }
    }
}
