using Microsoft.AspNetCore.Http;
using System;

namespace Digipolis.Correlation
{
    public interface ICorrelationContext
    {
        string Id { get; }
        string SourceId { get; }
        string SourceName { get; }
        string InstanceId { get; }
        string InstanceName { get; }
        string UserId { get; }
        string IpAddress { get; }
    }
}