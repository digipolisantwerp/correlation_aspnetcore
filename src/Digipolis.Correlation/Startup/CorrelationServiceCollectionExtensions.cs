using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Digipolis.Correlation.Helpers;

namespace Digipolis.Correlation
{
    public static class CorrelationServiceCollectionExtensions
    {
        public static IServiceCollection AddCorrelation(this IServiceCollection services, Action<CorrelationOptions> setupAction = null)
        {
            if (setupAction == null)
                setupAction = options => { };

            services.Configure<CorrelationOptions>(setupAction);
            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.TryAddTransient<ICorrelationService, CorrelationService>();
            services.TryAddTransient<ICorrelationContextFormatter, CorrelationContextFormatter>();
            services.TryAddTransient<CorrelationIdHandler>();
            services.TryAddScoped<IScopedCorrelationContext, ScopedCorrelationContext>();

            return services;
        }
    }
}
