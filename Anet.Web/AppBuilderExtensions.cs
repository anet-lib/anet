using Anet.Web.Api;
using Anet.Web.Jwt;

namespace Microsoft.AspNetCore.Builder;

public static class AppBuilderExtensions
{
    public static void UseAnetExceptionHandler(this IApplicationBuilder app, params string[] pathPrefixes)
    {
        app.UseMiddleware<ExceptionMiddleware>(pathPrefixes?.ToHashSet());
    }

    public static IApplicationBuilder UseAnetJwtAuthentication(this IApplicationBuilder app, string tokenPath)
    {
        app.UseMiddleware<JwtProviderMiddleware>(tokenPath);
        return app;
    }
}
