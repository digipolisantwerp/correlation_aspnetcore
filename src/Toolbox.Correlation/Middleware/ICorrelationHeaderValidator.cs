using System;

namespace Toolbox.Correlation.Middleware
{
    public interface ICorrelationHeaderValidator
    {
        bool IsValid(CorrelationHeader correlationHeader);
    }
}
