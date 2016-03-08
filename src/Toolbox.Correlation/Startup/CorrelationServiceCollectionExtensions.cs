using Microsoft.Extensions.DependencyInjection;
using System;

namespace Toolbox.Correlation
{
    public static class CorrelationServiceCollectionExtensions
    {
        public static IServiceCollection AddCorrelation(this IServiceCollection services, Action<CorrelationOptions> setupAction = null)
        {
            if (setupAction == null)
                setupAction = options => { };

            services.Configure(setupAction);
            services.AddScoped<ICorrelationContext, CorrelationContext>();

            return services;
        }
    }
}
