namespace Anet.Web.Jwt;

public interface IRefreshTokenStore
{
    Task SaveTokenAsync(JwtResult jwtResult);

    Task<string> GetTokenAsync(string refreshToken);

    Task DeleteTokenAsync(string refreshToken);
}

