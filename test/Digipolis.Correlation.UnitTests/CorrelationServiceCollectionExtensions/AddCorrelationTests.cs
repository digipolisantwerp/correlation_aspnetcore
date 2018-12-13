using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Linq;
using Xunit;

namespace Digipolis.Correlation.UnitTests.CorrelationIdServiceCollectionExtensions
{
    public class AddCorrelationTests
    {
        [Fact]
        private void CorrelationServiceIsRegistratedAsTransient()
        {
            var services = new ServiceCollection();
            services.AddCorrelation();

            var registrations = services.Where(sd => sd.ServiceType == typeof(ICorrelationService) && 
                                                     sd.ImplementationType == typeof(CorrelationService))
                                        .ToArray();

            Assert.Single(registrations);
            Assert.Equal(ServiceLifetime.Transient, registrations[0].Lifetime);
        }

        [Fact]
        private void CorrelationIdHandlerIsRegistratedAsTransient()
        {
            var services = new ServiceCollection();
            services.AddCorrelation();

            var registrations = services.Where(sd => sd.ServiceType == typeof(CorrelationIdHandler) &&
                                                     sd.ImplementationType == typeof(CorrelationIdHandler))
                                        .ToArray();

            Assert.Single(registrations);
            Assert.Equal(ServiceLifetime.Transient, registrations[0].Lifetime);
        }

        [Fact]
        private void CorrelationContextFormatterIsRegistratedAsTransient()
        {
            var services = new ServiceCollection();
            services.AddCorrelation();

            var registrations = services.Where(sd => sd.ServiceType == typeof(ICorrelationContextFormatter) &&
                                                     sd.ImplementationType == typeof(CorrelationContextFormatter))
                                        .ToArray();

            Assert.Single(registrations);
            Assert.Equal(ServiceLifetime.Transient, registrations[0].Lifetime);
        }

        [Fact]
        private void HttpContextAccessorIsRegistratedAsSingleton()
        {
            var services = new ServiceCollection();
            services.AddCorrelation();

            var registrations = services.Where(sd => sd.ServiceType == typeof(IHttpContextAccessor) &&
                                                     sd.ImplementationType == typeof(HttpContextAccessor))
                                        .ToArray();

            Assert.Single(registrations);
            Assert.Equal(ServiceLifetime.Singleton, registrations[0].Lifetime);
        }

        [Fact]
        private void CorrelationOptionsIsRegistratedAsSingleton()
        {
            var services = new ServiceCollection();
            services.AddCorrelation();

            var registrations = services.Where(sd => sd.ServiceType == typeof(IConfigureOptions<CorrelationOptions>))
                                        .ToArray();

            Assert.Single(registrations);
            Assert.Equal(ServiceLifetime.Singleton, registrations[0].Lifetime);
        }
    }
}
