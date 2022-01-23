using System.Text;
using Anet;
using Anet.Web.Api;
using Anet.Web.Jwt;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace Microsoft.Extensions.DependencyInjection;

public static class AnetBuilderExtensions
{
    public static AnetBuilder AddApi(
        this AnetBuilder builder,
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
            builder.Services.AddControllersWithViews(configMvcOptions);
        }
        else
        {
            builder.Services.AddControllers(configMvcOptions);
        }

        builder.Services.Configure<ApiBehaviorOptions>(apiBehaviorOptions =>
        {
            apiBehaviorOptions.SuppressModelStateInvalidFilter = true;
            configureApiBehavior?.Invoke(apiBehaviorOptions);
        });

        return builder;
    }

    public static AnetBuilder AddJwt<TAuthenticator>(
        this AnetBuilder builder,
        Action<JwtTokenOptions> configure)
        where TAuthenticator : class, IAuthenticator
    {
        return builder.AddJwt<TAuthenticator, DefaultRefreshTokenStore>(configure);
    }

    public static AnetBuilder AddJwt(
        this AnetBuilder builder, Action<JwtTokenOptions> configure)
    {
        return builder.AddJwt<NoopAuthenticator, DefaultRefreshTokenStore>(configure);
    }

    public static AnetBuilder AddJwt<TAuthenticator, TRefreshTokenStore>(
        this AnetBuilder builder,
        Action<JwtTokenOptions> configure,
        Action<JwtBearerOptions> configureJwtBearer = null)
        where TAuthenticator : class, IAuthenticator
        where TRefreshTokenStore : class, IRefreshTokenStore
    {
        var options = new JwtTokenOptions();
        configure(options);

        ArgumentNullException.ThrowIfNull(options);
        ArgumentNullException.ThrowIfNull(options.SigningKey);

        builder.Services.AddSingleton(options);
        builder.Services.AddSingleton<JwtProvider>();
        builder.Services.AddTransient<IAuthenticator, TAuthenticator>();
        builder.Services.AddTransient<IRefreshTokenStore, TRefreshTokenStore>();
        builder.Services.AddAuthentication(options =>
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
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.SigningKey)),
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

        return builder;
    }
}

