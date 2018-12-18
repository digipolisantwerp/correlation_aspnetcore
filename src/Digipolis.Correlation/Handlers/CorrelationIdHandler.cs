using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Digipolis.Correlation
{
    public class CorrelationIdHandler : DelegatingHandler
    {

        private readonly ICorrelationService _correlationService;

        public CorrelationIdHandler(ICorrelationService correlationService)
        {
            _correlationService = correlationService ?? throw new System.ArgumentNullException(nameof(correlationService));
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            request.Headers.TryAddWithoutValidation(CorrelationHeader.Key, _correlationService.GetContext().DgpHeader);
            return await base.SendAsync(request, cancellationToken);
        }
    }
}