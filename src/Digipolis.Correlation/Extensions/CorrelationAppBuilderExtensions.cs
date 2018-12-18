using Microsoft.AspNetCore.Builder;

namespace Digipolis.Correlation
{
    public static class CorrelationAppBuilderExtensions
    {
        public static IApplicationBuilder UseCorrelation(this IApplicationBuilder app)
        {
            app.UseMiddleware<CorrelationMiddleware>();

            return app;
        }
    }
}
