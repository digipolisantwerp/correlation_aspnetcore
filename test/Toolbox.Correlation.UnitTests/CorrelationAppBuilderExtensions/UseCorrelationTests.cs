using System;
using System.Collections.Generic;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Http.Features;
using Moq;
using Toolbox.Correlation;
using Xunit;

namespace Toolbox.Correlation.UnitTests.CorrelationIdAppBuilderExtensions
{
    public class UseCorrelationIdTests
    {
        [Fact]
        private void UseMiddlewareGetsCalled()
        {
            string source = "TestApp";
            string instance = "TestInstance";

            var app = new ApplicationBuilderMock();

            app.UseCorrelation(source, instance);

            Assert.True(app.UseMethodGotCalled);
        }
    }

    public class ApplicationBuilderMock : IApplicationBuilder
    {
        public bool UseMethodGotCalled { get; set; }

        public IServiceProvider ApplicationServices
        {
            get
            {
                return Mock.Of<IServiceProvider>();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public IDictionary<string, object> Properties
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public IFeatureCollection ServerFeatures
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public RequestDelegate Build()
        {
            throw new NotImplementedException();
        }

        public IApplicationBuilder New()
        {
            throw new NotImplementedException();
        }

        public IApplicationBuilder Use(Func<RequestDelegate, RequestDelegate> middleware)
        {
            UseMethodGotCalled = true;
            return this;
        }
    }
}
