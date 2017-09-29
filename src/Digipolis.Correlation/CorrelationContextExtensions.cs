using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Digipolis.Correlation
{
    public static class CorrelationContextExtensions
    {
        public static string CreateCorrelationHeaderData(this ICorrelationContext context)
        {
            string json = JsonConvert.SerializeObject(context);
            var jsonAsBytes = System.Text.Encoding.UTF8.GetBytes(json);
            return Convert.ToBase64String(jsonAsBytes);
        }

    }
}
