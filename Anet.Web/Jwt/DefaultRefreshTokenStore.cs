using Microsoft.AspNetCore.Http;

namespace Anet.Web.Jwt;

public class DefaultRefreshTokenStore : IRefreshTokenStore
{
    private readonly HttpContext _httpContext;

    public DefaultRefreshTokenStore(IHttpContextAccessor httpContextAccessor)
    {
        _httpContext = httpContextAccessor.HttpContext;
    }

    public Task SaveTokenAsync(JwtResult jwtResult)
    {
        return Task.CompletedTask;
    }

    public Task<string> GetTokenAsync(string refreshToken)
    {
        if (string.IsNullOrEmpty(refreshToken))
        {
            string prefix = "Bearer ";
            string token = _httpContext.Request.Headers["Authorization"];
            if (!string.IsNullOrEmpty(token) && token.StartsWith(prefix))
            {
                return Task.FromResult(token[prefix.Length..]);
            }
        }
        return Task.FromResult(default(string));
    }

    public Task DeleteTokenAsync(string refreshToken)
    {
        return Task.CompletedTask;
    }
}
