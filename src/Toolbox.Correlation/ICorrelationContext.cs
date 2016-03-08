using Microsoft.AspNet.Http;
using System;

namespace Toolbox.Correlation
{
    public interface ICorrelationContext
    {
        string CorrelationId { get; }
        string CorrelationSource { get; }
        string IdHeaderKey { get; }
        string SourceHeaderKey { get; }
    }
}