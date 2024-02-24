using System.Text;
using Anet.Web.Jwt;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServicesExtensions
{
    public static IServiceCollection AddAnetJwt(
        this IServiceCollection services,
        Action<JwtOptions> configure)
    {
        return services.AddAnetJwt<DefaultRefreshTokenStore>(configure);
    }

    public static IServiceCollection AddAnetJwt(
        this IServiceCollection services,
        Action<JwtOptions> configure,
        Action<JwtBearerOptions> configureJwtBearer)
    {
        return services.AddAnetJwt<DefaultRefreshTokenStore>(configure, configureJwtBearer);
    }

    public static IServiceCollection AddAnetJwt<TRefreshTokenStore>(
        this IServiceCollection services,
        Action<JwtOptions> configure,
        Action<JwtBearerOptions> configureJwtBearer = null)
        where TRefreshTokenStore : class, IRefreshTokenStore
    {
        var options = new JwtOptions();
        configure(options);

        ArgumentNullException.ThrowIfNull(options);
        ArgumentNullException.ThrowIfNull(options.Key);

        services.AddSingleton(options);
        services.AddHttpContextAccessor();
        services.AddTransient<JwtProvider>();
        services.AddTransient<IRefreshTokenStore, TRefreshTokenStore>();
        services
            .AddAuthentication(options =>
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
                    ValidateLifetime = options.Lifetime > 0,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.Key)),
                    ClockSkew = TimeSpan.FromSeconds(options.ClockSkew)
                };
                if (!string.IsNullOrEmpty(options.FallbackCookieKey))
                {
                    jwtBearerOptions.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            var token = context.Request.Cookies[options.FallbackCookieKey];
                            if (!string.IsNullOrEmpty(token))
                            {
                                context.Token = token;
                            }
                            return Task.CompletedTask;
                        }
                    };
                }
                configureJwtBearer?.Invoke(jwtBearerOptions);
            });

        return services;
    }
}

