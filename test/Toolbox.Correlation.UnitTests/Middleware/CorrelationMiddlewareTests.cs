using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Http.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.OptionsModel;
using Moq;
using Newtonsoft.Json;
using Toolbox.Correlation.Middleware;
using Toolbox.Correlation.UnitTests.Utilities;
using Xunit;

namespace Toolbox.Correlation.UnitTests.CorrelationId
{
    public class CorrelationMiddlewareTests
    {
        private readonly string _applicationSourceId = "thisApplicationId";
        private readonly string _applicationSourceName = "thisApplicationName";
        private readonly string _applicationInstanceId = "thisInstanceId";
        private readonly string _applicationInstanceName = "thisInstanceName";
        private readonly ICorrelationHeaderValidator _mockedCorrelationHeaderValidator = CreateMockedCorrelationHeaderValidator();
        private List<string> _loggedMessages;
        

        public CorrelationMiddlewareTests()
        {
            _loggedMessages = new List<string>();
        }

        [Fact]
        private void ThrowExceptionWhenNextDelegateIsNull()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => new CorrelationIdMiddleware(null, "sourceId", "sourceName", "instanceId", "instanceName", CreateLogger(_loggedMessages)));
            Assert.Equal("next", ex.ParamName);
        }

        [Fact]
        private void ThrowExceptionWhenSourceIdIsNull()
        {
            var requestDelegate = CreateRequestDelegate();
            var ex = Assert.Throws<ArgumentNullException>(() => new CorrelationIdMiddleware(requestDelegate, null, "sourceName", "instanceId", "instanceName", CreateLogger(_loggedMessages)));
            Assert.Equal("sourceId", ex.ParamName);
        }

        [Fact]
        private void ThrowExceptionWhenSourceIdIsEmpty()
        {
            var requestDelegate = CreateRequestDelegate();
            var ex = Assert.Throws<ArgumentNullException>(() => new CorrelationIdMiddleware(requestDelegate, String.Empty, "sourceName", "instanceId", "instanceName", CreateLogger(_loggedMessages)));
            Assert.Equal("sourceId", ex.ParamName);
        }

        [Fact]
        private void ThrowExceptionWhenSourceIdIsWhiteSpace()
        {
            var requestDelegate = CreateRequestDelegate();
            var ex = Assert.Throws<ArgumentNullException>(() => new CorrelationIdMiddleware(requestDelegate, "   ", "sourceName", "instanceId", "instanceName", CreateLogger(_loggedMessages)));
            Assert.Equal("sourceId", ex.ParamName);
        }

        [Fact]
        private void ThrowExceptionWhenSourceNameIsNull()
        {
            var requestDelegate = CreateRequestDelegate();
            var ex = Assert.Throws<ArgumentNullException>(() => new CorrelationIdMiddleware(requestDelegate, "sourceId", null, "instanceId", "instanceName", CreateLogger(_loggedMessages)));
            Assert.Equal("sourceName", ex.ParamName);
        }

        [Fact]
        private void ThrowExceptionWhenSourceNameIsEmpty()
        {
            var requestDelegate = CreateRequestDelegate();
            var ex = Assert.Throws<ArgumentNullException>(() => new CorrelationIdMiddleware(requestDelegate, "sourceId", String.Empty, "instanceId", "instanceName", CreateLogger(_loggedMessages)));
            Assert.Equal("sourceName", ex.ParamName);
        }

        [Fact]
        private void ThrowExceptionWhenSourceNameIsWhiteSpace()
        {
            var requestDelegate = CreateRequestDelegate();
            var ex = Assert.Throws<ArgumentNullException>(() => new CorrelationIdMiddleware(requestDelegate, "sourceId", "   ", "instanceId", "instanceName", CreateLogger(_loggedMessages)));
            Assert.Equal("sourceName", ex.ParamName);
        }

        [Fact]
        private void ThrowExceptionWhenInstanceIdIsNull()
        {
            var requestDelegate = CreateRequestDelegate();
            var ex = Assert.Throws<ArgumentNullException>(() => new CorrelationIdMiddleware(requestDelegate, "sourceId", "sourceName", null, "instanceName", CreateLogger(_loggedMessages)));
            Assert.Equal("instanceId", ex.ParamName);
        }

        [Fact]
        private void ThrowExceptionWhenInstanceIdIsEmpty()
        {
            var requestDelegate = CreateRequestDelegate();
            var ex = Assert.Throws<ArgumentNullException>(() => new CorrelationIdMiddleware(requestDelegate, "sourceId", "sourceName", String.Empty, "instanceName", CreateLogger(_loggedMessages)));
            Assert.Equal("instanceId", ex.ParamName);
        }

        [Fact]
        private void ThrowExceptionWhenInstanceIdIsWhiteSpace()
        {
            var requestDelegate = CreateRequestDelegate();
            var ex = Assert.Throws<ArgumentNullException>(() => new CorrelationIdMiddleware(requestDelegate, "sourceId", "sourceName", "   ", "instanceName", CreateLogger(_loggedMessages)));
            Assert.Equal("instanceId", ex.ParamName);
        }

        [Fact]
        private void ThrowExceptionWhenInstanceNameIsNull()
        {
            var requestDelegate = CreateRequestDelegate();
            var ex = Assert.Throws<ArgumentNullException>(() => new CorrelationIdMiddleware(requestDelegate, "sourceId", "sourceName", "instanceId", null, CreateLogger(_loggedMessages)));
            Assert.Equal("instanceName", ex.ParamName);
        }

        [Fact]
        private void ThrowExceptionWhenInstanceNameIsEmpty()
        {
            var requestDelegate = CreateRequestDelegate();
            var ex = Assert.Throws<ArgumentNullException>(() => new CorrelationIdMiddleware(requestDelegate, "sourceId", "sourceName", "instanceId", String.Empty, CreateLogger(_loggedMessages)));
            Assert.Equal("instanceName", ex.ParamName);
        }

        [Fact]
        private void ThrowExceptionWhenInstanceNameIsWhiteSpace()
        {
            var requestDelegate = CreateRequestDelegate();
            var ex = Assert.Throws<ArgumentNullException>(() => new CorrelationIdMiddleware(requestDelegate, "sourceId", "sourceName", "instanceId", "   ", CreateLogger(_loggedMessages)));
            Assert.Equal("instanceName", ex.ParamName);
        }

        [Fact]
        private void ThrowExceptionWhenLoggerIsNull()
        {
            var requestDelegate = CreateRequestDelegate();
            var ex = Assert.Throws<ArgumentNullException>(() => new CorrelationIdMiddleware(requestDelegate, "sourceId", "sourceName", "instanceId", "instanceName", null));
            Assert.Equal("logger", ex.ParamName);
        }

        [Fact]
        public async Task ThrowExceptionWhenCorrelationContextNotAvailable()
        {
            var requestDelegate = CreateRequestDelegate();
            var middleware = new CorrelationIdMiddleware(requestDelegate, _applicationSourceId, _applicationSourceName, _applicationInstanceId, _applicationInstanceName, CreateLogger(_loggedMessages));

            Exception x = await Assert.ThrowsAsync<ArgumentNullException>(() => middleware.Invoke(new DefaultHttpContext(), Options.Create(new CorrelationOptions()), _mockedCorrelationHeaderValidator));
        }

        [Fact]
        private async Task ThrowExceptionWhenCorrelationHeaderValidatorIsNull()
        {
            var httpContext = new DefaultHttpContext();
            var options = new CorrelationOptions();
            var correlationContext = CreateContext(options);

            httpContext.RequestServices = CreateServiceProvider(correlationContext, options);
            var requestDelegate = CreateRequestDelegate();
            var middleware = new CorrelationIdMiddleware(requestDelegate, _applicationSourceId, _applicationSourceName, _applicationInstanceId, _applicationInstanceName, CreateLogger(_loggedMessages));

            var ex = await Assert.ThrowsAnyAsync<ArgumentNullException>(() => middleware.Invoke(httpContext, Options.Create(options), null));

            Assert.Equal("correlationHeaderValidator", ex.ParamName);
        }

        [Fact]
        private async Task NextDelegateInvoked()
        {
            var httpContext = new DefaultHttpContext();
            var options = new CorrelationOptions();

            httpContext.RequestServices = CreateServiceProvider(CreateContext(options), options);

            var isInvoked = false;
            var requestDelegate = CreateRequestDelegate(() => isInvoked = true);

             var middleware = new CorrelationIdMiddleware(requestDelegate, _applicationSourceId, _applicationSourceName, _applicationInstanceId, _applicationInstanceName, CreateLogger(_loggedMessages));

            await middleware.Invoke(httpContext, Options.Create(options), _mockedCorrelationHeaderValidator);

            Assert.True(isInvoked);
        }

        [Fact]
        private async Task CreateNewCorrelationValuesWhenHeaderNotPresent()
        {
            var httpContext = new DefaultHttpContext();
            var options = new CorrelationOptions();
            var correlationContext = CreateContext(options);

            httpContext.RequestServices = CreateServiceProvider(correlationContext, options);
            var requestDelegate = CreateRequestDelegate();
             var middleware = new CorrelationIdMiddleware(requestDelegate, _applicationSourceId, _applicationSourceName, _applicationInstanceId, _applicationInstanceName, CreateLogger(_loggedMessages));

            await middleware.Invoke(httpContext, Options.Create(options), _mockedCorrelationHeaderValidator);

            Assert.NotEqual(new Guid().ToString(), correlationContext.Id);
            Assert.Equal(_applicationSourceId, correlationContext.SourceId);
            Assert.Equal(_applicationSourceName, correlationContext.SourceName);
            Assert.Equal(_applicationInstanceId, correlationContext.InstanceId);
            Assert.Equal(_applicationInstanceName, correlationContext.InstanceName);
        }

        [Fact]
        private async Task UseCorrelationValuesWhenHeaderPresent()
        {
            var correlationHeader = new CorrelationHeader() { Id = "anId", SourceId = "aSourceId", SourceName = "aSourceName", InstanceId = "anInstanceId", InstanceName = "anInstanceName" };
            var correlationHeaderBase64 = ToBase64(correlationHeader);

            var httpContext = new DefaultHttpContext();
            var options = new CorrelationOptions();
            var correlationContext = CreateContext(options);

            httpContext.RequestServices = CreateServiceProvider(correlationContext, options);
            httpContext.Request.Headers.Add(options.HeaderKey, correlationHeaderBase64);

            var requestDelegate = CreateRequestDelegate();
            var middleware = new CorrelationIdMiddleware(requestDelegate, _applicationSourceId, _applicationSourceName, _applicationInstanceId, _applicationInstanceName, CreateLogger(_loggedMessages));

            await middleware.Invoke(httpContext, Options.Create(options), _mockedCorrelationHeaderValidator);

            Assert.Equal(correlationHeader.Id, correlationContext.Id);
            Assert.Equal(correlationHeader.SourceId, correlationContext.SourceId);
            Assert.Equal(correlationHeader.SourceName, correlationContext.SourceName);
            Assert.Equal(correlationHeader.InstanceId, correlationContext.InstanceId);
            Assert.Equal(correlationHeader.InstanceName, correlationContext.InstanceName);
        }

        [Fact]
        private async Task ReturnsBadRequestWhenNotBase64Encoded()
        {
            var correlationHeader = new CorrelationHeader() { Id = "anId", SourceId = "aSourceId", SourceName = "aSourceName", InstanceId = "anInstanceId", InstanceName = "anInstanceName" };
            var correlationHeaderJson = JsonConvert.SerializeObject(correlationHeader);

            var httpContext = new DefaultHttpContext();
            var options = new CorrelationOptions();
            var correlationContext = CreateContext(options);

            httpContext.RequestServices = CreateServiceProvider(correlationContext, options);
            httpContext.Request.Headers.Add(options.HeaderKey, correlationHeaderJson);

            var requestDelegate = CreateRequestDelegate();
            var middleware = new CorrelationIdMiddleware(requestDelegate, _applicationSourceId, _applicationSourceName, _applicationInstanceId, _applicationInstanceName, CreateLogger(_loggedMessages));

            await middleware.Invoke(httpContext, Options.Create(options), _mockedCorrelationHeaderValidator);

            Assert.Equal(400, httpContext.Response.StatusCode);
        }

        [Fact]
        private async Task ReturnsBadRequestWhenInvalidHeader()
        {
            var correlationHeaderJson = "{\"this\":\"is\",\"not\":\"a\",\"valid\":\"CorrelationHeader\"}";
            var correlationHeaderBase64 = JsonToBase64(correlationHeaderJson);

            var httpContext = new DefaultHttpContext();
            var options = new CorrelationOptions();
            var correlationContext = CreateContext(options);

            httpContext.RequestServices = CreateServiceProvider(correlationContext, options);
            httpContext.Request.Headers.Add(options.HeaderKey, correlationHeaderBase64);

            var requestDelegate = CreateRequestDelegate();
            var middleware = new CorrelationIdMiddleware(requestDelegate, _applicationSourceId, _applicationSourceName, _applicationInstanceId, _applicationInstanceName, CreateLogger(_loggedMessages));

            var validator = new Mock<ICorrelationHeaderValidator>();
            validator.Setup((v) => v.IsValid(It.IsAny<CorrelationHeader>())).Returns(false);

            await middleware.Invoke(httpContext, Options.Create(options), validator.Object);

            Assert.Equal(400, httpContext.Response.StatusCode);
        }

        [Fact]
        private async Task ReturnsBadRequestWhenInvalidJson()
        {
            var correlationHeaderJson = "{\"this\"\"is\",\"not\":\"a\",,,\"valid\":\"json\"}";
            var correlationHeaderBase64 = JsonToBase64(correlationHeaderJson);

            var httpContext = new DefaultHttpContext();
            var options = new CorrelationOptions();
            var correlationContext = CreateContext(options);

            httpContext.RequestServices = CreateServiceProvider(correlationContext, options);
            httpContext.Request.Headers.Add(options.HeaderKey, correlationHeaderBase64);

            var requestDelegate = CreateRequestDelegate();
            var middleware = new CorrelationIdMiddleware(requestDelegate, _applicationSourceId, _applicationSourceName, _applicationInstanceId, _applicationInstanceName, CreateLogger(_loggedMessages));

            await middleware.Invoke(httpContext, Options.Create(options), _mockedCorrelationHeaderValidator);

            Assert.Equal(400, httpContext.Response.StatusCode);
        }

        [Fact]
        private async Task CorrelationValuesAreLogged()
        {
            var correlationHeader = new CorrelationHeader() { Id = "anId", SourceId = "aSourceId", SourceName = "aSourceName", InstanceId = "anInstanceId", InstanceName = "anInstanceName" };
            var correlationHeaderBase64 = ToBase64(correlationHeader);

            var httpContext = new DefaultHttpContext();
            var options = new CorrelationOptions();
            var correlationContext = CreateContext(options);

            httpContext.RequestServices = CreateServiceProvider(correlationContext, options);
            httpContext.Request.Headers.Add(options.HeaderKey, correlationHeaderBase64);

            var requestDelegate = CreateRequestDelegate();
            var middleware = new CorrelationIdMiddleware(requestDelegate, _applicationSourceId, _applicationSourceName, _applicationInstanceId, _applicationInstanceName, CreateLogger(_loggedMessages));

            await middleware.Invoke(httpContext, Options.Create(options), _mockedCorrelationHeaderValidator);

            Assert.Equal(4, _loggedMessages.Count);
            Assert.Equal($"Debug, CorrelationId: {correlationHeader.Id}", _loggedMessages[0]);
            Assert.Equal($"Debug, CorrelationSourceName: {correlationHeader.SourceName}", _loggedMessages[1]);
            Assert.Equal($"Debug, CorrelationInstanceName: {correlationHeader.InstanceName}", _loggedMessages[2]);
            Assert.Equal($"Debug, CorrelationUser: {correlationHeader.UserId}", _loggedMessages[3]);
        }
        
        private ICorrelationContext CreateContext(CorrelationOptions options)
        {
            var context = new CorrelationContext(Options.Create(options));
            return context;
        }

        private IServiceProvider CreateServiceProvider(ICorrelationContext context, CorrelationOptions options = null)
        {
            var serviceProviderMock = new Mock<IServiceProvider>();
            serviceProviderMock.Setup(p => p.GetService(typeof(ICorrelationContext))).Returns(context);

            if (options != null)
                serviceProviderMock.Setup(p => p.GetService(typeof(IOptions<CorrelationOptions>))).Returns(Options.Create(options));

            return serviceProviderMock.Object;
        }

        private ILogger<CorrelationIdMiddleware> CreateLogger(List<string> loggedMessages)
        {
            return new TestLogger<CorrelationIdMiddleware>(loggedMessages);
        }

        private RequestDelegate CreateRequestDelegate(Action action = null)
        {
            return new RequestDelegate(ctx =>
            {
                if (action != null) action.Invoke();
                return Task.FromResult(false);
            });
        }

        private Task CreateDefaultHttpContext(HttpContext HttpContext)
        {
            return Task.FromResult(false);
        }

        private string ToBase64(CorrelationHeader header)
        {
            var jsonHeader = JsonConvert.SerializeObject(header);
            var bytes = Encoding.UTF8.GetBytes(jsonHeader);
            var base64 = Convert.ToBase64String(bytes);
            return base64;
        }

        private string JsonToBase64(string jsonString)
        {
            var bytes = Encoding.UTF8.GetBytes(jsonString);
            var base64 = Convert.ToBase64String(bytes);
            return base64;
        }

        private static ICorrelationHeaderValidator CreateMockedCorrelationHeaderValidator()
        {
            var validator = new Mock<ICorrelationHeaderValidator>();
            validator.Setup(v => v.IsValid(It.IsAny<CorrelationHeader>())).Returns(true);
            return validator.Object;
        }
    }
}
