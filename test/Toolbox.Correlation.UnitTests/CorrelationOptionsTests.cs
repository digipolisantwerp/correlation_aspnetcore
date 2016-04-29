using System;
using Xunit;

namespace Toolbox.Correlation.UnitTests.CorrelationId
{
    public class CorrelationOptionsTests
    {
        [Fact]
        public void SetDefaultValuesOnCreation()
        {
            var options = new CorrelationOptions();

            Assert.Equal(CorrelationHeaderKeys.HeaderKey, options.HeaderKey);
        }
    }
}
