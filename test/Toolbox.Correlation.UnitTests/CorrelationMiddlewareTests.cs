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
using Toolbox.Correlation.UnitTests.Utilities;
using Xunit;

namespace Toolbox.Correlation.UnitTests.CorrelationId
{
    public class CorrelationMiddlewareTests
    {
        private readonly string _externalSource = "externalApplication";
        private readonly string _externalInstance = "externalInstance";
        private readonly string _applicationSource = "thisApplication";
        private readonly string _applicationInstance = "thisInstance";
        private List<string> _loggedMessages;

        public CorrelationMiddlewareTests()
        {
            _loggedMessages = new List<string>();
        }

        [Fact]
        private void ThrowExceptionWhenNextDelegateIsNull()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => new CorrelationIdMiddleware(null, "src", "instance", CreateLogger(_loggedMessages)));
            Assert.Equal("next", ex.ParamName);
        }

        [Fact]
        private void ThrowExceptionWhenSourceIsNull()
        {
            var requestDelegate = CreateRequestDelegate();
            var ex = Assert.Throws<ArgumentNullException>(() => new CorrelationIdMiddleware(requestDelegate, null, "instance", CreateLogger(_loggedMessages)));
            Assert.Equal("source", ex.ParamName);
        }

        [Fact]
        private void ThrowExceptionWhenSourceIsEmpty()
        {
            var requestDelegate = CreateRequestDelegate();
            var ex = Assert.Throws<ArgumentNullException>(() => new CorrelationIdMiddleware(requestDelegate, String.Empty, "instance", CreateLogger(_loggedMessages)));
            Assert.Equal("source", ex.ParamName);
        }

        [Fact]
        private void ThrowExceptionWhenInstanceIsNull()
        {
            var requestDelegate = CreateRequestDelegate();
            var ex = Assert.Throws<ArgumentNullException>(() => new CorrelationIdMiddleware(requestDelegate, "src", null, CreateLogger(_loggedMessages)));
            Assert.Equal("instance", ex.ParamName);
        }

        [Fact]
        private void ThrowExceptionWhenInstanceIsEmpty()
        {
            var requestDelegate = CreateRequestDelegate();
            var ex = Assert.Throws<ArgumentNullException>(() => new CorrelationIdMiddleware(requestDelegate, "src", String.Empty, CreateLogger(_loggedMessages)));
            Assert.Equal("instance", ex.ParamName);
        }

        [Fact]
        private void ThrowExceptionWhenLoggerIsNull()
        {
            var requestDelegate = CreateRequestDelegate();
            var ex = Assert.Throws<ArgumentNullException>(() => new CorrelationIdMiddleware(requestDelegate, "src", "instance", null));
            Assert.Equal("logger", ex.ParamName);
        }

        [Fact]
        public async void ThrowExceptionWhenCorrelationContextNotAvailable()
        {
            var requestDelegate = CreateRequestDelegate();
            var middleware = new CorrelationIdMiddleware(requestDelegate, _applicationSource, _applicationInstance, CreateLogger(_loggedMessages));

            Exception x = await Assert.ThrowsAsync<ArgumentNullException>(() => middleware.Invoke(new DefaultHttpContext(), Options.Create(new CorrelationOptions())));
        }

        [Fact]
        private async void NextDelegateInvoked()
        {
            var httpContext = new DefaultHttpContext();
            var options = new CorrelationOptions();

            httpContext.RequestServices = CreateServiceProvider(CreateContext(options), options);

            var isInvoked = false;
            var requestDelegate = CreateRequestDelegate(() => isInvoked = true);

             var middleware = new CorrelationIdMiddleware(requestDelegate, _applicationSource, _applicationInstance, CreateLogger(_loggedMessages));

            await middleware.Invoke(httpContext, Options.Create(options));

            Assert.True(isInvoked);
        }

        [Fact]
        private async void CreateNewCorrelationValuesIfNotPresent()
        {
            var httpContext = new DefaultHttpContext();
            var options = new CorrelationOptions();
            var correlationContext = CreateContext(options);

            httpContext.RequestServices = CreateServiceProvider(correlationContext, options);
            var requestDelegate = CreateRequestDelegate();
             var middleware = new CorrelationIdMiddleware(requestDelegate, _applicationSource, _applicationInstance, CreateLogger(_loggedMessages));

            await middleware.Invoke(httpContext, Options.Create(options));

            Assert.NotEqual(new Guid().ToString(), correlationContext.Id);
            Assert.Equal(_applicationSource, correlationContext.Source);
            Assert.Equal(_applicationInstance, correlationContext.Instance);
        }

        [Fact]
        private async void UseCorrelationValuesIfPresent()
        {
            var correlationHeader = new CorrelationHeader() { Id = Guid.NewGuid().ToString(), Source = "aSource", Instance = "anInstance" };
            var correlationHeaderBase64 = ToBase64(correlationHeader);

            var httpContext = new DefaultHttpContext();
            var options = new CorrelationOptions();
            var correlationContext = CreateContext(options);

            httpContext.RequestServices = CreateServiceProvider(correlationContext, options);
            httpContext.Request.Headers.Add(options.HeaderKey, correlationHeaderBase64);

            var requestDelegate = CreateRequestDelegate();
             var middleware = new CorrelationIdMiddleware(requestDelegate, _applicationSource, _applicationInstance, CreateLogger(_loggedMessages));

            await middleware.Invoke(httpContext, Options.Create(options));

            Assert.Equal(correlationHeader.Id, correlationContext.Id);
            Assert.Equal(correlationHeader.Source, correlationContext.Source);
            Assert.Equal(correlationHeader.Instance, correlationContext.Instance);
        }

        [Fact]
        private async void CorrelationValuesGetLogged()
        {
            var correlationHeader = new CorrelationHeader() { Id = Guid.NewGuid().ToString(), Source = "aSource", Instance = "anInstance", UserId = "aUser" };
            var correlationHeaderBase64 = ToBase64(correlationHeader);

            var httpContext = new DefaultHttpContext();
            var options = new CorrelationOptions();
            var correlationContext = CreateContext(options);

            httpContext.RequestServices = CreateServiceProvider(correlationContext, options);
            httpContext.Request.Headers.Add(options.HeaderKey, correlationHeaderBase64);

            var requestDelegate = CreateRequestDelegate();
            var middleware = new CorrelationIdMiddleware(requestDelegate, _applicationSource, _applicationInstance, CreateLogger(_loggedMessages));

            await middleware.Invoke(httpContext, Options.Create(options));

            Assert.Equal(4, _loggedMessages.Count);
            Assert.Equal($"Debug, CorrelationId: {correlationHeader.Id}", _loggedMessages[0]);
            Assert.Equal($"Debug, CorrelationSource: {correlationHeader.Source}", _loggedMessages[1]);
            Assert.Equal($"Debug, CorrelationInstance: {correlationHeader.Instance}", _loggedMessages[2]);
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
    }
}
