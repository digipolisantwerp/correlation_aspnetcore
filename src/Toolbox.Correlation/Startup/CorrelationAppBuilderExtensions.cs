using Microsoft.AspNet.Builder;

namespace Toolbox.Correlation
{
    public static class CorrelationAppBuilderExtensions
    {
        public static IApplicationBuilder UseCorrelation(this IApplicationBuilder app, string source, string instance)
        {
            app.UseMiddleware<CorrelationIdMiddleware>(source, instance);

            return app;
        }
    }
}
