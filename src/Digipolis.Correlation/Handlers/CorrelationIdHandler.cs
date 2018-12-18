using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Digipolis.Correlation
{
    public class CorrelationIdHandler : DelegatingHandler
    {
        private readonly string _correlationHeader;

        public CorrelationIdHandler(ICorrelationService correlationService)
        {
            if (correlationService == null) throw new System.ArgumentNullException(nameof(correlationService));
            _correlationHeader = correlationService.GetContext().DgpHeader;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            request.Headers.TryAddWithoutValidation(CorrelationHeader.Key, _correlationHeader);
            return await base.SendAsync(request, cancellationToken);
        }
    }
}