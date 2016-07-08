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
            correlationContext.TrySetValues(Guid.NewGuid().ToString(), "TestSource");

            client.SetCorrelationValues(correlationContext);

            Assert.NotNull(client.DefaultRequestHeaders.Single(h => h.Key == options.IdHeaderKey));
            Assert.Equal(correlationContext.CorrelationId.ToString(), client.DefaultRequestHeaders.Single(h => h.Key == options.IdHeaderKey).Value.Single());

            Assert.NotNull(client.DefaultRequestHeaders.Single(h => h.Key == options.SourceHeaderKey));
            Assert.Equal(correlationContext.CorrelationSource.ToString(), client.DefaultRequestHeaders.Single(h => h.Key == options.SourceHeaderKey).Value.Single());
        }

        [Fact]
        public void AddHeadersToClientWithServiceProvider()
        {
            var client = new HttpClient();
            var options = new CorrelationOptions();
            var correlationContext = new CorrelationContext(Utilities.Options.Create(options));
            correlationContext.TrySetValues(Guid.NewGuid().ToString(), "TestSource");

            client.SetCorrelationValues(CreateServiceProvider(correlationContext, options));

            Assert.NotNull(client.DefaultRequestHeaders.Single(h => h.Key == options.IdHeaderKey));
            Assert.Equal(correlationContext.CorrelationId.ToString(), client.DefaultRequestHeaders.Single(h => h.Key == options.IdHeaderKey).Value.Single());

            Assert.NotNull(client.DefaultRequestHeaders.Single(h => h.Key == options.SourceHeaderKey));
            Assert.Equal(correlationContext.CorrelationSource.ToString(), client.DefaultRequestHeaders.Single(h => h.Key == options.SourceHeaderKey).Value.Single());
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
