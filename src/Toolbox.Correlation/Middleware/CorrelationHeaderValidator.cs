using System;

namespace Toolbox.Correlation.Middleware
{
    public class CorrelationHeaderValidator : ICorrelationHeaderValidator
    {
        public bool IsValid(CorrelationHeader correlationHeader)
        {
            return ( !String.IsNullOrWhiteSpace(correlationHeader.Id) &&
                     !String.IsNullOrWhiteSpace(correlationHeader.SourceId) &&
                     !String.IsNullOrWhiteSpace(correlationHeader.SourceName) &&
                     !String.IsNullOrWhiteSpace(correlationHeader.InstanceId) &&
                     !String.IsNullOrWhiteSpace(correlationHeader.InstanceName) );
        }
    }
}
