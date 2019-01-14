using Anet.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;

namespace Microsoft.AspNetCore.Builder
{
    public static class AnetApplicationBuilder
    {
        public static IApplicationBuilder UseAnet(this IApplicationBuilder app)
        {
            var loggerFactory = app.ApplicationServices.GetRequiredService<ILoggerFactory>();
            loggerFactory.AddNLog();

            Database.Logger = app.ApplicationServices.GetRequiredService<ILogger<Database>>();

            return app;
        }
    }
}
