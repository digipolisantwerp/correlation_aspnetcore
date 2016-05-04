using System;
using Toolbox.Correlation.Middleware;
using Xunit;

namespace Toolbox.Correlation.UnitTests.Middleware
{
    public class CorrelationHeaderValidatorTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        void IdInvalidReturnsFalse(string id)
        {
            var header = new CorrelationHeader() { Id = id, SourceId = "sourceId", SourceName = "sourceName", InstanceId = "instanceId", InstanceName = "instanceName" };
            var validator = new CorrelationHeaderValidator();
            var isValid = validator.IsValid(header);
            Assert.False(isValid);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        void SourceIdInvalidReturnsFalse(string sourceId)
        {
            var header = new CorrelationHeader() { Id = "id", SourceId = sourceId, SourceName = "sourceName", InstanceId = "instanceId", InstanceName = "instanceName" };
            var validator = new CorrelationHeaderValidator();
            var isValid = validator.IsValid(header);
            Assert.False(isValid);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        void SourceNameInvalidReturnsFalse(string sourceName)
        {
            var header = new CorrelationHeader() { Id = "id", SourceId = "sourceId", SourceName = sourceName, InstanceId = "instanceId", InstanceName = "instanceName" };
            var validator = new CorrelationHeaderValidator();
            var isValid = validator.IsValid(header);
            Assert.False(isValid);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        void InstanceIdInvalidReturnsFalse(string instanceId)
        {
            var header = new CorrelationHeader() { Id = "id", SourceId = "sourceId", SourceName = "sourceName", InstanceId = instanceId, InstanceName = "instanceName" };
            var validator = new CorrelationHeaderValidator();
            var isValid = validator.IsValid(header);
            Assert.False(isValid);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        void InstanceNameInvalidReturnsFalse(string instanceName)
        {
            var header = new CorrelationHeader() { Id = "id", SourceId = "sourceId", SourceName = "sourceName", InstanceId = "instanceId", InstanceName = instanceName };
            var validator = new CorrelationHeaderValidator();
            var isValid = validator.IsValid(header);
            Assert.False(isValid);
        }

        [Fact]
        void MandatoryFieldsCorrectReturnsTrue()
        {
            var header = new CorrelationHeader() { Id = "id", SourceId = "sourceId", SourceName = "sourceName", InstanceId = "instanceId", InstanceName = "instanceName" };
            var validator = new CorrelationHeaderValidator();
            var isValid = validator.IsValid(header);
            Assert.True(isValid);
        }
    }
}
