using System;

namespace Toolbox.Correlation
{
    public interface ICorrelationContext
    {
        string Id { get; }
        string SourceId { get; }
        string SourceName { get; }
        string InstanceId { get; }
        string InstanceName { get; }
        string UserId { get; }
        string IPAddress { get; }

        string UserToken { get; }

        string HeaderName { get; }
    }
}