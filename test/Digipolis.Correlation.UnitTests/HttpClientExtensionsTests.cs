using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Digipolis.Correlation.UnitTests.Utilities;
using Xunit;

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
        public void AddHeadersToClientWithCorrelationContext()
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

            client.SetCorrelationValues(correlationContext);

            Assert.NotNull(client.DefaultRequestHeaders.Single(h => h.Key == options.IdHeaderKey));
            Assert.Equal(correlationContext.Id, client.DefaultRequestHeaders.Single(h => h.Key == options.IdHeaderKey).Value.Single());

            Assert.NotNull(client.DefaultRequestHeaders.Single(h => h.Key == options.SourceHeaderKey));
            Assert.Equal(correlationContext.SourceId.ToString(), client.DefaultRequestHeaders.Single(h => h.Key == options.SourceHeaderKey).Value.Single());
        }

        [Fact]
        public void AddHeadersToClientWithServiceProvider()
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

            Assert.NotNull(client.DefaultRequestHeaders.Single(h => h.Key == options.IdHeaderKey));
            Assert.Equal(correlationContext.Id.ToString(), client.DefaultRequestHeaders.Single(h => h.Key == options.IdHeaderKey).Value.Single());

            Assert.NotNull(client.DefaultRequestHeaders.Single(h => h.Key == options.SourceHeaderKey));
            Assert.Equal(correlationContext.SourceId.ToString(), client.DefaultRequestHeaders.Single(h => h.Key == options.SourceHeaderKey).Value.Single());
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
