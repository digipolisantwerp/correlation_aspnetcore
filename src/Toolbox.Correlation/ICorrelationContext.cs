using System;

namespace Toolbox.Correlation
{
    public interface ICorrelationContext
    {
        string Id { get; }
        string Source { get; }
        string Instance { get; }
        string UserId { get; }
        string IPAddress { get; }
        string UserToken { get; }

        string HeaderKey { get; }
    }
}