using Newtonsoft.Json;
using System;
using System.Text;

namespace Digipolis.Correlation
{
    internal static class CorrelationContextExtensions
    {
        public static void SetDgpHeader (this CorrelationContext context)
        {
            if (context == null) throw new NullReferenceException($"{nameof(context)} cannot be null.");

            context.DgpHeader = Convert.ToBase64String(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(context)));
        }
    }
}
