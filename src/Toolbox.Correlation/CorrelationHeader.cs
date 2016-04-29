using System;

namespace Toolbox.Correlation
{
    public class CorrelationHeader
    {
        public string Id { get; set; }
        public string Source { get; set; }
        public string Instance { get; set; }
        public string UserId { get; set; }
        public string IPAddress { get; set; }
        public string UserToken { get; set; }
    }
}
