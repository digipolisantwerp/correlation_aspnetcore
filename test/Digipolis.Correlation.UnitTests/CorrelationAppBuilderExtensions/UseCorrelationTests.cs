using Microsoft.AspNetCore.Builder;
using Moq;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Http;

namespace Digipolis.Correlation.UnitTests.CorrelationIdAppBuilderExtensions
{
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
