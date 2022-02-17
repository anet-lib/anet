using System.Text;
using Anet.Web.Api;
using Anet.Web.Jwt;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServicesExtensions
{
    public static IServiceCollection AddAnetApi(
        this IServiceCollection services,
        Action<MvcOptions> configureMvc = null, 
        Action<ApiBehaviorOptions> configureApiBehavior = null, 
        bool withViews = false)
    {
        void configMvcOptions(MvcOptions mvcOptions)
        {
            mvcOptions.Filters.Add<ModelValidationFilterAttribute>();
            mvcOptions.Filters.Add<ApiResultFilterAttribute>();
            configureMvc?.Invoke(mvcOptions);
        };

        if (withViews)
        {
            services.AddControllersWithViews(configMvcOptions);
        }
        else
        {
            services.AddControllers(configMvcOptions);
        }

        services.Configure<ApiBehaviorOptions>(apiBehaviorOptions =>
        {
            apiBehaviorOptions.SuppressModelStateInvalidFilter = true;
            configureApiBehavior?.Invoke(apiBehaviorOptions);
        });

        return services;
    }

    public static IServiceCollection AddAnetJwt<TAuthenticator>(
        this IServiceCollection services,
        Action<JwtTokenOptions> configure)
        where TAuthenticator : class, IAuthenticator
    {
        return services.AddAnetJwt<TAuthenticator, DefaultRefreshTokenStore>(configure);
    }

    public static IServiceCollection AddAnetJwt(
        this IServiceCollection services, Action<JwtTokenOptions> configure)
    {
        return services.AddAnetJwt<NoopAuthenticator, DefaultRefreshTokenStore>(configure);
    }

    public static IServiceCollection AddAnetJwt<TAuthenticator, TRefreshTokenStore>(
        this IServiceCollection services,
        Action<JwtTokenOptions> configure,
        Action<JwtBearerOptions> configureJwtBearer = null)
        where TAuthenticator : class, IAuthenticator
        where TRefreshTokenStore : class, IRefreshTokenStore
    {
        var options = new JwtTokenOptions();
        configure(options);

        ArgumentNullException.ThrowIfNull(options);
        ArgumentNullException.ThrowIfNull(options.Key);

        services.AddSingleton(options);
        services.AddSingleton<JwtProvider>();
        services.AddTransient<IAuthenticator, TAuthenticator>();
        services.AddTransient<IRefreshTokenStore, TRefreshTokenStore>();
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
            .AddJwtBearer(jwtBearerOptions =>
            {
                jwtBearerOptions.TokenValidationParameters = new()
                {
                    ValidIssuer = options.Issuer,
                    ValidateIssuer = !string.IsNullOrEmpty(options.Issuer),
                    ValidAudience = options.Audience,
                    ValidateAudience = !string.IsNullOrEmpty(options.Audience),
                    ValidateLifetime = options.Expiration > 0,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.Key)),
                };
                jwtBearerOptions.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        context.Token = context.Request.Cookies[options.FallbackCookieKey];
                        return Task.CompletedTask;
                    }
                };
                configureJwtBearer?.Invoke(jwtBearerOptions);
            });

        return services;
    }
}

