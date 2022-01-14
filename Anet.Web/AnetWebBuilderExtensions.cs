using Anet.Web;
using Anet.Web.Api;
using Anet.Web.Jwt;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Microsoft.Extensions.DependencyInjection;

public static class AnetWebBuilderExtensions
{
    public static AnetWebBuilder AddAnetWebApi(this IServiceCollection services, Action<MvcOptions> configure = null)
    {
        var builder = new AnetWebBuilder(services);
        builder.MvcBuilder = builder.Services.AddControllers(options =>
        {
            options.Filters.Add<ModelValidationFilterAttribute>();
            options.Filters.Add<ApiResultFilterAttribute>();
            configure?.Invoke(options);
        });
        builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        return builder;
    }

    public static AnetWebBuilder AddJwtAuthentication<TAuthenticator, TRefreshTokenStore>(
        this AnetWebBuilder builder, Action<JwtOptions> configure)
        where TAuthenticator : class, IAuthenticator
        where TRefreshTokenStore : class, IRefreshTokenStore
    {
        var options = new JwtOptions();
        configure(options);

        if (string.IsNullOrEmpty(options.SigningKey))
        {
            throw new ArgumentNullException(nameof(options.SigningKey));
        }

        builder.Services.AddSingleton(options);
        builder.Services.AddSingleton<JwtProvider>();
        builder.Services.AddTransient<IAuthenticator, TAuthenticator>();
        builder.Services.AddTransient<IRefreshTokenStore, TRefreshTokenStore>();

        builder.AuthenticationBuilder = builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(opt =>
        {
            opt.TokenValidationParameters = new TokenValidationParameters
            {
                ValidIssuer = options.Issuer,
                ValidateIssuer = !string.IsNullOrEmpty(options.Issuer),
                ValidAudience = options.Audience,
                ValidateAudience = !string.IsNullOrEmpty(options.Audience),
                ValidateLifetime = options.Expiration > 0,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.SigningKey)),
            };
            opt.Events = new JwtBearerEvents
            {
                OnMessageReceived = context =>
                {
                    context.Token = context.Request.Cookies["Authorization"];
                    return Task.CompletedTask;
                }
            };
        });

        return builder;
    }

    public static AnetWebBuilder AddJwtAuthentication<TAuthenticator>(
        this AnetWebBuilder builder, Action<JwtOptions> configure)
        where TAuthenticator : class, IAuthenticator
    {
        return builder.AddJwtAuthentication<TAuthenticator, DefaultRefreshTokenStore>(configure);
    }

    public static AnetWebBuilder AddJwtAuthentication(
        this AnetWebBuilder builder, Action<JwtOptions> configure)
    {
        return builder.AddJwtAuthentication<NoopAuthenticator, DefaultRefreshTokenStore>(configure);
    }
}

