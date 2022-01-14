namespace Anet.Web.Jwt;

public interface IAuthenticator
{
    /// <summary>
    /// JWT 授权验证
    /// </summary>
    /// <param name="jwtParams">请求参数</param>
    /// <returns>验证结果</returns>
    Task<AuthenticateResult> AuthenticateAsync(JwtParams jwtParams);
}

