using Digipolis.ApplicationServices;
using Digipolis.Errors.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using Xunit;

namespace Digipolis.Correlation.UnitTests
{
    public class CorrelationMiddlewareTests
    {
        [Fact]
        public void ThrowExceptionWhenRequestDelegateIsNull()
        {
            var loggerMock = new Moq.Mock<ILogger<CorrelationMiddleware>>();
            var correlationService = new Moq.Mock<ICorrelationService>();
            var correlationFormatter = new Moq.Mock<ICorrelationContextFormatter>();
            var options = Options.Create<CorrelationOptions>(new CorrelationOptions() { CorrelationHeaderRequired = true });
            Assert.Throws<ArgumentNullException>(() => new CorrelationMiddleware(null, 
                logger: loggerMock.Object, 
                correlationService: correlationService.Object, 
                options: options, 
                correlationContextFormatter: correlationFormatter.Object));
        }

        [Fact]
        public void ThrowExceptionWhenLoggerIsNull()
        {
            var correlationService = new Moq.Mock<ICorrelationService>();
            var correlationFormatter = new Moq.Mock<ICorrelationContextFormatter>();
            var options = Options.Create<CorrelationOptions>(new CorrelationOptions() { CorrelationHeaderRequired = true });
            Assert.Throws<ArgumentNullException>(() => new CorrelationMiddleware(next: async (innerHttpContext) => { await innerHttpContext.Response.WriteAsync("test response body"); }, 
                logger:null, 
                correlationService: correlationService.Object, 
                options: options, 
                correlationContextFormatter: correlationFormatter.Object));
        }


        [Fact]
        public void ThrowExceptionWhenCorrelationServiceIsNull()
        {
            var loggerMock = new Moq.Mock<ILogger<CorrelationMiddleware>>();
            var correlationFormatter = new Moq.Mock<ICorrelationContextFormatter>();
            var options = Options.Create<CorrelationOptions>(new CorrelationOptions() { CorrelationHeaderRequired = true });
            Assert.Throws<ArgumentNullException>(() => new CorrelationMiddleware(next: async (innerHttpContext) => { await innerHttpContext.Response.WriteAsync("test response body"); }, 
                logger: loggerMock.Object, 
                correlationService: null, 
                options: options, 
                correlationContextFormatter: correlationFormatter.Object));
        }

        [Fact]
        public void ThrowExceptionWhenOptionsIsNull()
        {
            var loggerMock = new Moq.Mock<ILogger<CorrelationMiddleware>>();
            var correlationService = new Moq.Mock<ICorrelationService>();
            var correlationFormatter = new Moq.Mock<ICorrelationContextFormatter>();
            Assert.Throws<ArgumentNullException>(() => new CorrelationMiddleware(next: async (innerHttpContext) => { await innerHttpContext.Response.WriteAsync("test response body"); },
                logger: loggerMock.Object,
                correlationService: correlationService.Object,
                options: null,
                correlationContextFormatter: correlationFormatter.Object));
        }

        [Fact]
        public void ThrowExceptionWhenCorrelationFormatterIsNull()
        {
            var loggerMock = new Moq.Mock<ILogger<CorrelationMiddleware>>();
            var correlationService = new Moq.Mock<ICorrelationService>();
            var options = Options.Create<CorrelationOptions>(new CorrelationOptions() { CorrelationHeaderRequired = true });
            Assert.Throws<ArgumentNullException>(() => new CorrelationMiddleware(next: async (innerHttpContext) => { await innerHttpContext.Response.WriteAsync("test response body"); },
                logger: loggerMock.Object,
                correlationService: correlationService.Object,
                options: options,
                correlationContextFormatter: null));
        }

        [Fact]
        public void DontThrowExceptionWhenNoneIsNull()
        {
            var loggerMock = new Moq.Mock<ILogger<CorrelationMiddleware>>();
            var applicationContext = new Moq.Mock<IApplicationContext>();
            var correlationService = new Moq.Mock<ICorrelationService>();
            var correlationFormatter = new Moq.Mock<ICorrelationContextFormatter>();
            var options = Options.Create<CorrelationOptions>(new CorrelationOptions() { CorrelationHeaderRequired = true });
            Assert.NotNull(new CorrelationMiddleware(next: async (innerHttpContext) => { await innerHttpContext.Response.WriteAsync("test response body"); }, 
                logger: loggerMock.Object, 
                correlationService: correlationService.Object, 
                options: options, 
                correlationContextFormatter: correlationFormatter.Object));
        }

        [Fact]
        public void ThrowsValidationExceptionIfDgpHeaderRequiredAndMissing()
        {
            //Arrange
            var loggerMock = new Moq.Mock<ILogger<CorrelationMiddleware>>();
            var applicationContext = new Moq.Mock<IApplicationContext>();
            var correlationService = new Moq.Mock<ICorrelationService>();
            var correlationFormatter = new Moq.Mock<ICorrelationContextFormatter>();
            var options = Options.Create<CorrelationOptions>(new CorrelationOptions() { CorrelationHeaderRequired = true });
            var middleware = new CorrelationMiddleware(next: async (innerHttpContext) => { await innerHttpContext.Response.WriteAsync("test response body"); }, logger: loggerMock.Object, correlationService: correlationService.Object, options: options, correlationContextFormatter: correlationFormatter.Object);
            var correlationContext = new CorrelationContext();
            var httpContext = new DefaultHttpContext
            {
                RequestServices = new Moq.Mock<IServiceProvider>().Object
            };

            //Act
            Assert.ThrowsAsync<ValidationException>(() => middleware.Invoke(httpContext));
        }

        [Fact]
        public void ThrowsNoValidationExceptionIfDgpHeaderNotRequiredAndMissing()
        {
            //Arrange
            var loggerMock = new Moq.Mock<ILogger<CorrelationMiddleware>>();
            var applicationContext = new Moq.Mock<IApplicationContext>();
            var correlationService = new Moq.Mock<ICorrelationService>();
            var options = Options.Create<CorrelationOptions>(new CorrelationOptions() { CorrelationHeaderRequired = false });
            var correlationFormatter = new Moq.Mock<ICorrelationContextFormatter>();
            var middleware = new CorrelationMiddleware(next: async (innerHttpContext) => { await innerHttpContext.Response.WriteAsync("test response body"); }, logger: loggerMock.Object, correlationService: correlationService.Object, options: options, correlationContextFormatter: correlationFormatter.Object);
            var httpContext = new DefaultHttpContext
            {
                RequestServices = new Moq.Mock<IServiceProvider>().Object
            };

            var serviceProvider = new Moq.Mock<IServiceProvider>();

            //Act
            middleware.Invoke(httpContext);
        }

        [Fact]
        public void ThrowsValidationExceptionIfDgpHeaderRequiredAndInvalidIdInHeader()
        {
            //Arrange
            var loggerMock = new Moq.Mock<ILogger<CorrelationMiddleware>>();
            var applicationContext = new Moq.Mock<IApplicationContext>();
            var correlationService = new Moq.Mock<ICorrelationService>();
            var options = Options.Create<CorrelationOptions>(new CorrelationOptions() { CorrelationHeaderRequired = true });
            var correlationFormatter = new Moq.Mock<ICorrelationContextFormatter>();
            var middleware = new CorrelationMiddleware(next: async (innerHttpContext) => { await innerHttpContext.Response.WriteAsync("test response body"); }, logger: loggerMock.Object, correlationService: correlationService.Object, options: options, correlationContextFormatter: correlationFormatter.Object);
            var httpContext = new DefaultHttpContext
            {
                RequestServices = new Moq.Mock<IServiceProvider>().Object
            };
            httpContext.Request.Headers.Add(CorrelationHeader.Key, "eyJpZCI6bnVsbCwKInNvdXJjZUlkIjoiMSIsCiJzb3VyY2VOYW1lIjoidGVzdG5hbWUiLAoiaW5zdGFuY2VJZCI6IjIiLAoiaW5zdGFuY2VOYW1lIjoidGVzdGluc3RhbmNlIiwKdXNlcklkIjoidW5rb3dudXNlciIsCiJpcEFkZHJlc3MiOiIxMjMuMTIzLjEyMy4xMjMifQ==");

            var serviceProvider = new Moq.Mock<IServiceProvider>();

            //Act
            middleware.Invoke(httpContext);
        }

        [Fact]
        public void ThrowsValidationExceptionIfDgpHeaderRequiredAndInvalidHeader()
        {
            //Arrange
            var loggerMock = new Moq.Mock<ILogger<CorrelationMiddleware>>();
            var applicationContext = new Moq.Mock<IApplicationContext>();
            var correlationService = new Moq.Mock<ICorrelationService>();
            var options = Options.Create<CorrelationOptions>(new CorrelationOptions() { CorrelationHeaderRequired = true });
            var correlationFormatter = new Moq.Mock<ICorrelationContextFormatter>();
            var middleware = new CorrelationMiddleware(next: async (innerHttpContext) => { await innerHttpContext.Response.WriteAsync("test response body"); }, logger: loggerMock.Object, correlationService: correlationService.Object, options: options, correlationContextFormatter: correlationFormatter.Object);
            var httpContext = new DefaultHttpContext
            {
                RequestServices = new Moq.Mock<IServiceProvider>().Object
            };
            httpContext.Request.Headers.Add(CorrelationHeader.Key, "eyJibGFibGEiOiIxIiwKImJsYWJsYWJsYSI6InRlc3RuYW1lIn0");

            var serviceProvider = new Moq.Mock<IServiceProvider>();

            //Act
            middleware.Invoke(httpContext);
        }

        [Fact]
        public void ThrowsNoValidationExceptionIfDgpHeaderRequiredAndHeaderIsValid()
        {
            //Arrange
            var loggerMock = new Moq.Mock<ILogger<CorrelationMiddleware>>();
            var applicationContext = new Moq.Mock<IApplicationContext>();
            var correlationService = new Moq.Mock<ICorrelationService>();
            var options = Options.Create<CorrelationOptions>(new CorrelationOptions() { CorrelationHeaderRequired = true });
            var correlationFormatter = new Moq.Mock<ICorrelationContextFormatter>();
            var middleware = new CorrelationMiddleware(next: async (innerHttpContext) => { await innerHttpContext.Response.WriteAsync("test response body"); }, logger: loggerMock.Object, correlationService: correlationService.Object, options: options, correlationContextFormatter: correlationFormatter.Object);
            var httpContext = new DefaultHttpContext
            {
                RequestServices = new Moq.Mock<IServiceProvider>().Object
            };
            httpContext.Request.Headers.Add(CorrelationHeader.Key, "eyJpZCI6ImNvcnJlbGF0aW9uaWQiLAoic291cmNlSWQiOiIxIiwKInNvdXJjZU5hbWUiOiJ0ZXN0bmFtZSIsCiJpbnN0YW5jZUlkIjoiMiIsCiJpbnN0YW5jZU5hbWUiOiJ0ZXN0aW5zdGFuY2UiLAp1c2VySWQiOiJ1bmtvd251c2VyIiwKImlwQWRkcmVzcyI6IjEyMy4xMjMuMTIzLjEyMyJ9");

            var serviceProvider = new Moq.Mock<IServiceProvider>();

            //Act
            middleware.Invoke(httpContext);
        }

        [Fact]
        public void ThrowsNoValidationExceptionIfDgpHeaderRequired_Missing_AndMatches_CorrelationHeaderNotRequiredRouteRegex()
        {
            //Arrange
            var loggerMock = new Moq.Mock<ILogger<CorrelationMiddleware>>();
            var applicationContext = new Moq.Mock<IApplicationContext>();
            var correlationService = new Moq.Mock<ICorrelationService>();
            var options = Options.Create<CorrelationOptions>(new CorrelationOptions() { CorrelationHeaderRequired = true });
            var correlationFormatter = new Moq.Mock<ICorrelationContextFormatter>();
            var middleware = new CorrelationMiddleware(next: async (innerHttpContext) => { await innerHttpContext.Response.WriteAsync("test response body"); }, logger: loggerMock.Object, correlationService: correlationService.Object, options: options, correlationContextFormatter: correlationFormatter.Object);
            var httpContext = new DefaultHttpContext
            {
                RequestServices = new Moq.Mock<IServiceProvider>().Object
            };
            httpContext.Request.Path = "/v2/status/ping";

            var serviceProvider = new Moq.Mock<IServiceProvider>();

            //Act
            middleware.Invoke(httpContext);
        }
    }
}
