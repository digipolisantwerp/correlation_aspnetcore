using System;
using System.Linq;
using System.Net.Http;
using Microsoft.Extensions.OptionsModel;
using Moq;
using Toolbox.Correlation.UnitTests.Utilities;
using Xunit;

namespace Toolbox.Correlation.UnitTests.CorrelationId
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
            var correlationContext = new CorrelationContext(Options.Create(options));
            correlationContext.TrySetValues(Guid.NewGuid().ToString(), "TestSourceId", "TestSourceName", "TestInstanceId", "TestInstanceName");

            client.SetCorrelationValues(correlationContext);

            Assert.NotNull(client.DefaultRequestHeaders.Single(h => h.Key == options.HeaderKey));
            Assert.Equal(correlationContext.Id.ToString(), client.DefaultRequestHeaders.Single(h => h.Key == options.HeaderKey).Value.Single());
        }

        [Fact]
        public void AddHeadersToClientWithServiceProvider()
        {
            var client = new HttpClient();
            var options = new CorrelationOptions();
            var correlationContext = new CorrelationContext(Options.Create(options));
            correlationContext.TrySetValues(Guid.NewGuid().ToString(), "TestSourceId", "TestSourceName", "TestInstanceId", "TestInstanceName");

            client.SetCorrelationValues(CreateServiceProvider(correlationContext, options));

            Assert.NotNull(client.DefaultRequestHeaders.Single(h => h.Key == options.HeaderKey));
            Assert.Equal(correlationContext.Id.ToString(), client.DefaultRequestHeaders.Single(h => h.Key == options.HeaderKey).Value.Single());
        }

        private IServiceProvider CreateServiceProvider(ICorrelationContext context, CorrelationOptions options = null)
        {
            var serviceProviderMock = new Mock<IServiceProvider>();
            serviceProviderMock.Setup(p => p.GetService(typeof(ICorrelationContext))).Returns(context);

            if (options != null)
                serviceProviderMock.Setup(p => p.GetService(typeof(IOptions<CorrelationOptions>))).Returns(Options.Create(options));

            return serviceProviderMock.Object;
        }
    }
}
