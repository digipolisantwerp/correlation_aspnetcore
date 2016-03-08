using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Toolbox.Correlation.UnitTests.Utilities
{
    public class TestLogger<T> : ILogger<T>
    {
        private List<string> _loggedMessages;

        public TestLogger(List<string> loggedMessages)
        {
            _loggedMessages = loggedMessages;
        }

        public IDisposable BeginScopeImpl(object state)
        {
            throw new NotImplementedException();
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public void Log(LogLevel logLevel, int eventId, object state, Exception exception, Func<object, Exception, string> formatter)
        {
            _loggedMessages.Add($"{logLevel}, {state}");
        }
    }
}
