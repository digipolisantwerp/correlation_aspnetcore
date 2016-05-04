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

            Assert.Equal(options.HeaderKey, context.HeaderName);
        }

        [Fact]
        public void SetValuesFirstTime()
        {
            var options = new CorrelationOptions();
            var context = new CorrelationContext(Options.Create(options));
            var correlationId = "initialId";
            var sourceId = "initialSourceId";
            var sourceName = "initialSourceName";
            var instanceId = "initialInstanceId";
            var instanceName = "initialInstanceName";
            var userid = "initialUserId";
            var ipaddress = "initialIPAddress";
            var userToken = "initialUserToken";

            var result = context.TrySetValues(correlationId, sourceId, sourceName, instanceId, instanceName, userid, ipaddress, userToken);

            Assert.True(result);
            Assert.Equal(correlationId, context.Id);
            Assert.Equal(sourceId, context.SourceId);
            Assert.Equal(sourceName, context.SourceName);
            Assert.Equal(instanceId, context.InstanceId);
            Assert.Equal(instanceName, context.InstanceName);
            Assert.Equal(userid, context.UserId);
            Assert.Equal(ipaddress, context.IPAddress);
            Assert.Equal(userToken, context.UserToken);
        }

        [Fact]
        void IdIsNotOverwrittenWhenInitialized()
        {
            var options = new CorrelationOptions();
            var context = new CorrelationContext(Options.Create(options));
            var correlationId = "initialId";
            var sourceId = "initialSourceId";
            var sourceName = "initialSourceName";
            var instanceId = "initialInstanceId";
            var instanceName = "initialInstanceName";
            var userid = "initialUserId";
            var ipaddress = "initialIPAddress";
            var userToken = "initialUserToken";

            context.SetValues(correlationId, sourceId, sourceName, instanceId, instanceName, userid, ipaddress, userToken);
            context.SetValues("otherId", "otherSourceId", "otherSourceName", "otherInstanceId", "otherInstanceName", "otherUserId", "otherIPAddress", "otherUserToken");

            Assert.Equal(correlationId, context.Id);
        }

        [Fact]
        void SourceValuesAreNotOverwrittenWhenIdIsInitialized()
        {
            var options = new CorrelationOptions();
            var context = new CorrelationContext(Options.Create(options));
            var correlationId = "initialId";
            var sourceId = "initialSourceId";
            var sourceName = "initialSourceName";
            var instanceId = "initialInstanceId";
            var instanceName = "initialInstanceName";
            var userid = "initialUserId";
            var ipaddress = "initialIPAddress";
            var userToken = "initialUserToken";

            context.SetValues(correlationId, sourceId, sourceName, instanceId, instanceName, userid, ipaddress, userToken);
            context.SetValues("otherId", "otherSourceId", "otherSourceName", "otherInstanceId", "otherInstanceName", "otherUserId", "otherIPAddress", "otherUserToken");

            Assert.Equal(sourceId, context.SourceId);
            Assert.Equal(sourceName, context.SourceName);
            Assert.Equal(instanceId, context.InstanceId);
            Assert.Equal(instanceName, context.InstanceName);
        }

        [Fact]
        void UserValuesAreOverwrittenWhenIdIsInitialized()
        {
            var options = new CorrelationOptions();
            var context = new CorrelationContext(Options.Create(options));
            var correlationId = "initialId";
            var sourceId = "initialSourceId";
            var sourceName = "initialSourceName";
            var instanceId = "initialInstanceId";
            var instanceName = "initialInstanceName";
            var userid = "initialUserId";
            var ipaddress = "initialIPAddress";
            var userToken = "initialUserToken";

            context.SetValues(correlationId, sourceId, sourceName, instanceId, instanceName, userid, ipaddress, userToken);
            context.SetValues("otherId", "otherSourceId", "otherSourceName", "otherInstanceId", "otherInstanceName", "otherUserId", "otherIPAddress", "otherUserToken");

            Assert.Equal("otherUserId", context.UserId);
            Assert.Equal("otherIPAddress", context.IPAddress);
        }

        [Fact]
        void UserTokenIsNotOverwrittenWhenInitialized()
        {
            var options = new CorrelationOptions();
            var context = new CorrelationContext(Options.Create(options));
            var correlationId = "initialId";
            var sourceId = "initialSourceId";
            var sourceName = "initialSourceName";
            var instanceId = "initialInstanceId";
            var instanceName = "initialInstanceName";
            var userid = "initialUserId";
            var ipaddress = "initialIPAddress";
            var userToken = "initialUserToken";

            context.SetValues(correlationId, sourceId, sourceName, instanceId, instanceName, userid, ipaddress, userToken);
            context.SetValues("otherId", "otherSourceId", "otherSourceName", "otherInstanceId", "otherInstanceName", "otherUserId", "otherIPAddress", "otherUserToken");

            Assert.Equal(userToken, context.UserToken);
        }
    }
}
