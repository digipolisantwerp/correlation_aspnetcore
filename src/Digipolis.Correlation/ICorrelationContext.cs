using Microsoft.AspNetCore.Http;
using System;

namespace Digipolis.Correlation
{
    public interface ICorrelationContext
    {
        string CorrelationId { get; }
        string CorrelationSource { get; }
        string IdHeaderKey { get; }
        string SourceHeaderKey { get; }
    }
}