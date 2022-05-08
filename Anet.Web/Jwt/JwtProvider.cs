using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Anet.Web.Jwt;

public class JwtProvider
{
    private readonly JwtOptions _options;
    private readonly IRefreshTokenStore _refreshTokenStore;

    public JwtProvider(JwtOptions options, IRefreshTokenStore refreshTokenStore)
    {
        _options = options;
        _refreshTokenStore = refreshTokenStore;
    }

    public async Task<JwtResult> GenerateToken(IEnumerable<Claim> claims)
    {
        var expires = DateTime.UtcNow.AddSeconds(_options.Lifetime);

        var jwtSecurityToken = new JwtSecurityToken(
            claims: claims,
            issuer: _options.Issuer,
            audience: _options.Audience,
            expires: _options.Lifetime > 0 ? expires : null,
            signingCredentials: new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Key)),
                SecurityAlgorithms.HmacSha256)
        );

        var accessToken = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);

        var result = new JwtResult
        {
            AccessToken = accessToken,
            ExpiresAt = expires.ToTimestamp()
        };

        await _refreshTokenStore.SaveTokenAsync(result);

        return result;
    }

    public async Task<JwtResult> RefreshToken(string refreshToken)
    {
        var token = await _refreshTokenStore.GetTokenAsync(refreshToken);
        if (token == null) return null;

        var securityToken = new JwtSecurityTokenHandler().ReadJwtToken(token);
        if (securityToken == null || securityToken.ValidTo < DateTime.Now)
            return null;
        var newToken = await GenerateToken(securityToken.Claims.ToList());

        await _refreshTokenStore.DeleteTokenAsync(refreshToken);
        await _refreshTokenStore.SaveTokenAsync(newToken);

        return newToken;
    }
}

