using Digipolis.Correlation.UnitTests.Utilities;
using Digipolis.Errors.Exceptions;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Digipolis.Correlation.UnitTests.CorrelationId
{
    public class CorrelationContextFormatterTests
    {

        [Fact]
        public void ThrowExceptionWhenLoggerIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new CorrelationContextFormatter(null));
        }

       
        [Fact]
        public void DontThrowExceptionWhenNoneIsNull()
        {
            var logger = new Moq.Mock<ILogger<CorrelationContextFormatter>>().Object;
            Assert.NotNull(new CorrelationContextFormatter(logger));
        }

        [Fact]
        public void SetPropertiesFromDgpHeaderReturnsCorrelationContext()
        {
            var context = new CorrelationContext
            {
                Id = Guid.NewGuid().ToString(),
                SourceId = Guid.NewGuid().ToString(),
                SourceName = "appName",
                InstanceId = Guid.NewGuid().ToString(),
                InstanceName = "appName-instanceName",
                UserId = "userId",
                IpAddress = "194.25.76.122"
            };
            context.SetDgpHeader();
            var logger = new Moq.Mock<ILogger<CorrelationContextFormatter>>().Object;
            var contextFormatter = new CorrelationContextFormatter(logger);

            var result = contextFormatter.ValidateAndSetPropertiesFromDgpHeader(context.DgpHeader);
          
            Assert.Equal(result.Id, context.Id);
            Assert.Equal(result.SourceId, context.SourceId);
            Assert.Equal(result.SourceName, context.SourceName);
            Assert.Equal(result.InstanceId, context.InstanceId);
            Assert.Equal(result.InstanceName, context.InstanceName);
            Assert.Equal(result.UserId, context.UserId);
            Assert.Equal(result.IpAddress, context.IpAddress);
        }

        [Fact]
        public void SetPropertiesFromDgpHeaderWithIdNullReturnsValidationException()
        {
            var context = new CorrelationContext
            {
                Id = null,
                SourceId = Guid.NewGuid().ToString(),
                SourceName = "appName",
                InstanceId = Guid.NewGuid().ToString(),
                InstanceName = "appName-instanceName",
                UserId = "userId",
                IpAddress = "194.25.76.122"
            };
            context.SetDgpHeader();
            var contextFormatter = new CorrelationContextFormatter(new TestLogger<CorrelationContextFormatter>(new List<string>()));

            var ex = Assert.Throws<ValidationException>(() => contextFormatter.ValidateAndSetPropertiesFromDgpHeader(context.DgpHeader));
            Assert.Contains("Invalid correlationheader, id is required", ex.Message);

            Assert.Contains(context.DgpHeader, String.Join(",", ex.Messages.SelectMany(x => x.Value.Select(y => y.ToString()))));
            Assert.Equal(ErrorCode.InvalidCorrelationHeader, ex.Code);
        }

        [Fact]
        public void SetPropertiesFromDgpHeaderWithInvalidHeaderReturnsValidationException()
        {
            var contextFormatter = new CorrelationContextFormatter(new TestLogger<CorrelationContextFormatter>(new List<string>()));
            var header = "someheader";

            var ex = Assert.Throws<ValidationException>(() => contextFormatter.ValidateAndSetPropertiesFromDgpHeader(header));
            Assert.Contains("Invalid correlationheader", ex.Message);
            Assert.Contains(header, String.Join(",", ex.Messages.SelectMany(x => x.Value.Select(y => y.ToString()))));
            Assert.Equal(ErrorCode.InvalidCorrelationHeader, ex.Code);
        }
    }
}
