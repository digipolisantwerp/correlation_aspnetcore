﻿using Microsoft.AspNet.Http;
using Microsoft.Extensions.OptionsModel;
using System;

namespace Toolbox.Correlation
{
    public class CorrelationContext : ICorrelationContext
    {
        private readonly CorrelationOptions _options;

        public CorrelationContext(IOptions<CorrelationOptions> options)
        {
            if (options.Value == null) throw new ArgumentNullException(nameof(CorrelationOptions), $"{nameof(CorrelationOptions)} cannot be null.");

            _options = options.Value;

            IdHeaderKey = _options.IdHeaderKey;
            SourceHeaderKey = _options.SourceHeaderKey;
        }

        public string CorrelationId { get; private set; }
        public string CorrelationSource { get; private set; }
        public string IdHeaderKey { get; private set; } 
        public string SourceHeaderKey { get; private set; }

        internal bool TrySetValues(string id, string source)
        {
            if (String.IsNullOrWhiteSpace(CorrelationId))
            {
                CorrelationId = id;
                CorrelationSource = source;
                return true;
            }

            return false;
        }
    }
}
