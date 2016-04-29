using System;
using Toolbox.Correlation.UnitTests.Utilities;
using Xunit;

namespace Toolbox.Correlation.UnitTests.CorrelationId
{
    public class CorrelationContextTests
    {
        [Fact]
        private void ThrowExceptionWhenOptionsIsNull()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => new CorrelationContext(Options.Create<CorrelationOptions>(null)));
        }

        [Fact]
        private void SetHeaderKeysFromOptions()
        {
            var options = new CorrelationOptions();

            var context = new CorrelationContext(Options.Create(options));

            Assert.Equal(options.HeaderKey, context.HeaderKey);
        }

        [Fact]
        public void SetValuesFirstTime()
        {
            var options = new CorrelationOptions();
            var context = new CorrelationContext(Options.Create(options));
            var correlationId = Guid.NewGuid().ToString();
            var correlationSource = "TestSource";
            var correlationInstance = "TestInstance";
            var userid = "userid";
            var ipaddress = "ipaddress";
            var usertoken = "usertoken";

            var result = context.TrySetValues(correlationId, correlationSource, correlationInstance, userid, ipaddress, usertoken);

            Assert.True(result);
            Assert.Equal(correlationId, context.Id);
            Assert.Equal(correlationSource, context.Source);
            Assert.Equal(correlationInstance, context.Instance);
            Assert.Equal(userid, context.UserId);
            Assert.Equal(ipaddress, context.IPAddress);
            Assert.Equal(usertoken, context.UserToken);
        }

        [Fact]
        public void KeepFirstTimeValues()
        {
            var options = new CorrelationOptions();
            var context = new CorrelationContext(Options.Create(options));
            var correlationId = Guid.NewGuid().ToString();
            var correlationSource = "TestSource";
            var correlationInstance = "TestInstance";
            var userid = "userid";
            var ipaddress = "ipaddress";
            var usertoken = "usertoken";

            context.TrySetValues(correlationId, correlationSource, correlationInstance, userid, ipaddress, usertoken);
            var result = context.TrySetValues(Guid.NewGuid().ToString(), "otherSource", "otherInstance", "otherUserid", "otherIPAddress", "otherUserToken");

            Assert.False(result);
            Assert.Equal(correlationId, context.Id);
            Assert.Equal(correlationSource, context.Source);
            Assert.Equal(correlationInstance, context.Instance);
            Assert.Equal(userid, context.UserId);
            Assert.Equal(ipaddress, context.IPAddress);
            Assert.Equal(usertoken, context.UserToken);
        }
    }
}
