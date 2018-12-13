using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Digipolis.Correlation;
using Xunit;

namespace Digipolis.Correlation.UnitTests.CorrelationId
{
    public class CorrelationOptionsTests
    {
        [Fact]
        public void SetDefaultValuesOnCreation()
        {
            var options = new CorrelationOptions();

            Assert.False(options.CorrelationHeaderRequired);
        }
    }
}
