using Microsoft.AspNet.Builder;

namespace Toolbox.Correlation
{
    public static class CorrelationAppBuilderExtensions
    {
        public static IApplicationBuilder UseCorrelation(this IApplicationBuilder app, string source)
        {
            app.UseMiddleware<CorrelationIdMiddleware>(source);

            return app;
        }
    }
}
