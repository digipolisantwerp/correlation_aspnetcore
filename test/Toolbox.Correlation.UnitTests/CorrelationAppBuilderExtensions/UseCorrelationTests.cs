using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Builder.Internal;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Toolbox.Correlation;
using Xunit;
using Microsoft.AspNet.Http.Features;

namespace Toolbox.Correlation.UnitTests.CorrelationIdAppBuilderExtensions
{
    public class UseCorrelationIdTests
    {
        [Fact]
        private void UseMiddlewareGetsCalled()
        {
            string source = "TestApp";

            var app = new ApplicationBuilderMock();

            app.UseCorrelation(source);

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
