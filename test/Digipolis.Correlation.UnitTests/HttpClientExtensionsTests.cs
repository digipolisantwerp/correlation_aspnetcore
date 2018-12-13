using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Digipolis.Correlation.UnitTests.Utilities;
using Xunit;
using System.Text;
using Newtonsoft.Json.Linq;

namespace Digipolis.Correlation.UnitTests.CorrelationId
{
    public class HttpClientExtensionsTests
    {
        [Fact]
        public void ThrowsException()
        {
            var client = new HttpClient();
            ICorrelationContext context = null;

            Assert.Throws<NullReferenceException>(() => client.SetCorrelationValues(context));
        }

        [Fact]
        public void AddsCorrelationHeaderWithContext()
        {
            var client = new HttpClient();
            var options = new CorrelationOptions();
            var correlationContext = new CorrelationContext(Utilities.Options.Create(options));

            var id = Guid.NewGuid().ToString();
            var sourceId = Guid.NewGuid().ToString();
            var sourceName = "testSource";
            var instanceId = Guid.NewGuid().ToString();
            var instanceName = "testSource-instanceName";
            var userId = "userId";
            var ipAddress = "194.25.76.122";

            correlationContext.TrySetValues(id, sourceId, sourceName, instanceId, instanceName, userId, ipAddress);

            client.SetCorrelationValues(CreateServiceProvider(correlationContext, options));

            var result = client.DefaultRequestHeaders.Single(h => h.Key == CorrelationHeaders.HeaderKey);
            Assert.NotEqual(default(KeyValuePair<string, IEnumerable<string>>), result);
            byte[] data = Convert.FromBase64String(result.Value.Single());
            string json = Encoding.UTF8.GetString(data);

            dynamic parsedHeader = JObject.Parse(json);
            string correlationId = (string)parsedHeader.id;

            Assert.Equal(id, (string)parsedHeader.id);
            Assert.Equal(sourceId, (string)parsedHeader.sourceId);
            Assert.Equal(sourceName, (string)parsedHeader.sourceName);
            Assert.Equal(instanceId, (string)parsedHeader.instanceId);
            Assert.Equal(instanceName, (string)parsedHeader.instanceName);
            Assert.Equal(userId, (string)parsedHeader.userId);
            Assert.Equal(ipAddress, (string)parsedHeader.ipAddress);
        }

        private IServiceProvider CreateServiceProvider(ICorrelationContext context, CorrelationOptions options = null)
        {
            var serviceProviderMock = new Mock<IServiceProvider>();
            serviceProviderMock.Setup(p => p.GetService(typeof(ICorrelationContext))).Returns(context);

            if (options != null)
                serviceProviderMock.Setup(p => p.GetService(typeof(IOptions<CorrelationOptions>))).Returns(Utilities.Options.Create(options));

            return serviceProviderMock.Object;
        }
    }
}
