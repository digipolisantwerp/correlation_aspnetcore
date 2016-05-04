using System;

namespace Toolbox.Correlation
{
    public class CorrelationHeader
    {
        public string Id { get; set; }
        public string SourceId { get; set; }
        public string SourceName { get; set; }
        public string InstanceId { get; set; }
        public string InstanceName { get; set; }
        public string UserId { get; set; }
        public string IPAddress { get; set; }
        public string UserToken { get; set; }
    }
}
