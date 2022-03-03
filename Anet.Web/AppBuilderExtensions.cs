using Anet.Web.Api;
using Anet.Web.Jwt;

namespace Microsoft.AspNetCore.Builder;

public static class AppBuilderExtensions
{
    public static void UseAnetExceptionHandler(this IApplicationBuilder app, params string[] pathPrefixes)
    {
        app.UseMiddleware<ExceptionMiddleware>(pathPrefixes?.ToHashSet());
    }

    /// <summary>
    /// Adds the to the <see cref="JwtProviderMiddleware"/> and <see cref="Authentication.AuthenticationMiddleware"/> to the
    /// specified <see cref="IApplicationBuilder"/>.
    /// </summary>
    /// <param name="app">The <see cref="IApplicationBuilder"/> to add the middleware to.</param>
    /// <param name="tokenPath">The api path used to get the token.</param>
    /// <returns></returns>
    public static IApplicationBuilder UseAnetJwtAuthentication(this IApplicationBuilder app, string tokenPath)
    {
        app.UseMiddleware<JwtProviderMiddleware>(tokenPath);
        app.UseAuthentication();
        return app;
    }
}
