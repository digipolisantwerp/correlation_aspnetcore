using Digipolis.ApplicationServices;
using Digipolis.Errors.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Digipolis.Correlation.UnitTests
{
    public class CorrelationMiddlewareTests
    {
        [Fact]
        public void FillCorrelationContextSetsCorrelationContextPropertiesWithBase64EncodedCorrelationHeader()
        {
            //Arrange
            var loggerMock = new Moq.Mock<ILogger<CorrelationMiddleware>>();
            var applicationContext = new Moq.Mock<IApplicationContext>();
            var middleware = new CorrelationMiddleware(next: async (innerHttpContext) => { await innerHttpContext.Response.WriteAsync("test response body"); }, logger: loggerMock.Object, applicationContext: applicationContext.Object);
            var dgpheader = "eyJpZCI6ImlkNDU2NDIzMTI0IiwNCiJzb3VyY2VJZCI6InRlc3Rzb3VyY2VJZCIsDQoic291cmNlTmFtZSI6InRlc3RTb3VyY2VOYW1lIiwNCiJpbnN0YW5jZUlkIjoidGVzdEluc3RhbmNlSWQiLA0KImluc3RhbmNlTmFtZSI6InRlc3RJbnN0YW5jZU5hbWUiLA0KInVzZXJJZCI6InRlc3RVc2VySWQiLA0KImlwQWRkcmVzcyI6IjEyMy4xMjMuMTIzLjEyMyJ9";
            var options = Options.Create<CorrelationOptions>(new CorrelationOptions());
            var correlationContext = new CorrelationContext(options);

            //Act
            middleware.FillCorrelationContext(dgpheader, correlationContext);

            //Assert
            Assert.Equal("id456423124",correlationContext.Id);
            Assert.Equal("testsourceId", correlationContext.SourceId);
            Assert.Equal("testSourceName", correlationContext.SourceName);
            Assert.Equal("testInstanceId", correlationContext.InstanceId);
            Assert.Equal("testInstanceName", correlationContext.InstanceName);
            Assert.Equal("testUserId", correlationContext.UserId);
            Assert.Equal("123.123.123.123", correlationContext.IpAddress);
            Assert.Equal(dgpheader, correlationContext.DgpHeader);
        }

        [Fact]
        public void ThrowsValidationExceptionIfDgpHeaderRequiredAndMissing()
        {
            //Arrange
            var loggerMock = new Moq.Mock<ILogger<CorrelationMiddleware>>();
            var applicationContext = new Moq.Mock<IApplicationContext>();
            var middleware = new CorrelationMiddleware(next: async (innerHttpContext) => { await innerHttpContext.Response.WriteAsync("test response body"); }, logger: loggerMock.Object, applicationContext: applicationContext.Object);
            var options = Options.Create<CorrelationOptions>(new CorrelationOptions() { CorrelationHeaderRequired = true });
            var correlationContext = new CorrelationContext(options);
            var httpContext = new DefaultHttpContext();
            httpContext.RequestServices = new Moq.Mock<IServiceProvider>().Object;

            //Act
            Assert.ThrowsAsync<ValidationException>(() => middleware.Invoke(httpContext, options));
        }

        [Fact]
        public void ThrowsNoValidationExceptionIfDgpHeaderRequired_Missing_AndMatches_CorrelationHeaderNotRequiredRouteRegex()
        {
            //Arrange
            var loggerMock = new Moq.Mock<ILogger<CorrelationMiddleware>>();
            var applicationContext = new Moq.Mock<IApplicationContext>();
            var middleware = new CorrelationMiddleware(next: async (innerHttpContext) => { await innerHttpContext.Response.WriteAsync("test response body"); }, logger: loggerMock.Object, applicationContext: applicationContext.Object);
            var options = Options.Create<CorrelationOptions>(new CorrelationOptions() { CorrelationHeaderRequired = true });
            var correlationContext = new CorrelationContext(options);
            var httpContext = new DefaultHttpContext();
            httpContext.RequestServices = new Moq.Mock<IServiceProvider>().Object;
            httpContext.Request.Path = "/v2/status/ping";

            var serviceProvider = new Moq.Mock<IServiceProvider>();
            serviceProvider.Setup((x) => x.GetService(typeof(ICorrelationContext))).Returns(correlationContext);
            httpContext.RequestServices = serviceProvider.Object;

            //Act
            middleware.Invoke(httpContext, options);
        }

        [Fact]
        public void FillsCorrelationContextWithApplicationContextIfHeaderNotRequiredAndNotProvided()
        {
            //Arrange
            var loggerMock = new Moq.Mock<ILogger<CorrelationMiddleware>>();
            var applicationContext = new ApplicationContext("appId", "appName");

            var middleware = new CorrelationMiddleware(next: async (innerHttpContext) => { await innerHttpContext.Response.WriteAsync("test response body"); }, logger: loggerMock.Object, applicationContext: applicationContext);
            var options = Options.Create<CorrelationOptions>(new CorrelationOptions() { CorrelationHeaderRequired = false });
            var correlationContext = new CorrelationContext(options);
            var httpContext = new DefaultHttpContext();

            var serviceProvider = new Moq.Mock<IServiceProvider>();
            serviceProvider.Setup((x) => x.GetService(typeof(ICorrelationContext))).Returns(correlationContext);

            httpContext.RequestServices = serviceProvider.Object;

            //Act
            middleware.Invoke(httpContext, options);

            //Assert
            Assert.Equal("appId", correlationContext.SourceId);
        }

    }
}
