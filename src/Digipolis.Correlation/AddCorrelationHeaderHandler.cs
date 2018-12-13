using Microsoft.AspNetCore.Http;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Digipolis.Correlation
{
    public class AddCorrelationHeaderHandler : DelegatingHandler
    {
        private readonly string _correlation;

        public AddCorrelationHeaderHandler(IHttpContextAccessor context)
        {
            _correlation = context.HttpContext.Items["Dgp-Correlation"].ToString();
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            request.Headers.Add("Dgp-Correlation", _correlation);

            return await base.SendAsync(request, cancellationToken);
        }
    }
}
