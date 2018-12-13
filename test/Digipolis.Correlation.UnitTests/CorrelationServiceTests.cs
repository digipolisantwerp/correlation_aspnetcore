using Digipolis.ApplicationServices;
using Digipolis.Correlation.UnitTests.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using Xunit;

namespace Digipolis.Correlation.UnitTests.CorrelationId
{
    public class CorrelationServiceTests
    {
        [Fact]
        public void ThrowExceptionWhenHttpContextAccessorIsNull()
        {
            var applicationContext = new Moq.Mock<IApplicationContext>().Object;
            var logger = new Moq.Mock<ILogger<CorrelationService>>().Object;
            var correlationContextFormatter = new Moq.Mock<ICorrelationContextFormatter>().Object;
            Assert.Throws<ArgumentNullException>(() => new CorrelationService(null, applicationContext, logger, correlationContextFormatter));
        }

        [Fact]
        public void ThrowExceptionWhenApplicationContextIsNull()
        {
            var httpContextAccessor = new Moq.Mock<IHttpContextAccessor>().Object;
            var logger = new Moq.Mock<ILogger<CorrelationService>>().Object;
            var correlationContextFormatter = new Moq.Mock<ICorrelationContextFormatter>().Object;
            Assert.Throws<ArgumentNullException>(() => new CorrelationService(httpContextAccessor, null, logger, correlationContextFormatter));
        }

        [Fact]
        public void ThrowExceptionWhenLoggerIsNull()
        {
            var httpContextAccessor = new Moq.Mock<IHttpContextAccessor>().Object;
            var applicationContext = new Moq.Mock<IApplicationContext>().Object;
            var correlationContextFormatter = new Moq.Mock<ICorrelationContextFormatter>().Object;
            Assert.Throws<ArgumentNullException>(() => new CorrelationService(httpContextAccessor, applicationContext, null, correlationContextFormatter));
        }

        [Fact]
        public void ThrowExceptionWhenCorrelationContextFormatterIsNull()
        {
            var httpContextAccessor = new Moq.Mock<IHttpContextAccessor>().Object;
            var applicationContext = new Moq.Mock<IApplicationContext>().Object;
            var logger = new Moq.Mock<ILogger<CorrelationService>>().Object;
            Assert.Throws<ArgumentNullException>(() => new CorrelationService(httpContextAccessor, applicationContext, logger, null));
        }

        [Fact]
        public void DontThrowExceptionWhenNoneIsNull()
        {
            var httpContextAccessor = new Moq.Mock<IHttpContextAccessor>().Object;
            var applicationContext = new Moq.Mock<IApplicationContext>().Object;
            var logger = new Moq.Mock<ILogger<CorrelationService>>().Object;
            var correlationContextFormatter = new Moq.Mock<ICorrelationContextFormatter>().Object;
            Assert.NotNull(new CorrelationService(httpContextAccessor, applicationContext, logger, correlationContextFormatter));
        }

        [Fact]
        public void WhenDuplicateHeaderCorrelationContextIsCreated()
        {
            var httpContext = new DefaultHttpContext
            {
                RequestServices = new Moq.Mock<IServiceProvider>().Object
            };

            var httpContextAccessor = new Moq.Mock<IHttpContextAccessor>();
            httpContextAccessor.SetupProperty<HttpContext>(x => x.HttpContext, httpContext);
            var applicationContext = new Moq.Mock<IApplicationContext>().Object;
            var logger = new TestLogger<CorrelationService>(new List<string>());
            var correlationContextFormatter = new Moq.Mock<ICorrelationContextFormatter>();

            var service = new CorrelationService(httpContextAccessor.Object, applicationContext, logger, correlationContextFormatter.Object);
            var context = service.GetContext();

            Assert.NotNull(context.DgpHeader);
            Assert.NotNull(context.Id);
        }

        [Fact]
        public void WhenHeaderIsPresentExistingCorrelationContextIsAvailable()
        {
            var httpContext = new DefaultHttpContext
            {
                RequestServices = new Moq.Mock<IServiceProvider>().Object
            };
            var header = "eyJpZCI6ImNvcnJlbGF0aW9uaWQiLAoic291cmNlSWQiOiIxIiwKInNvdXJjZU5hbWUiOiJ0ZXN0bmFtZSIsCiJpbnN0YW5jZUlkIjoiMiIsCiJpbnN0YW5jZU5hbWUiOiJ0ZXN0aW5zdGFuY2UiLAp1c2VySWQiOiJ1bmtvd251c2VyIiwKImlwQWRkcmVzcyI6IjEyMy4xMjMuMTIzLjEyMyJ9";
            httpContext.Request.Headers.Add(CorrelationHeader.Key, header);

            var httpContextAccessor = new Moq.Mock<IHttpContextAccessor>();
            httpContextAccessor.SetupProperty<HttpContext>(x => x.HttpContext, httpContext);
            var applicationContext = new Moq.Mock<IApplicationContext>().Object;
            var logger = new Moq.Mock<ILogger<CorrelationService>>().Object;
            var correlationContextFormatter = new Moq.Mock<ICorrelationContextFormatter>();
            correlationContextFormatter.Setup(x => x.ValidateAndSetPropertiesFromDgpHeader(header)).Returns(new CorrelationContext { DgpHeader = header });
               
            
            var service = new CorrelationService(httpContextAccessor.Object, applicationContext, logger, correlationContextFormatter.Object);
            var context = service.GetContext();


            Assert.Equal(header, context.DgpHeader);
        }
    }
}
