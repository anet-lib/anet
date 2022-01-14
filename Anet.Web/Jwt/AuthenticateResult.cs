using System.Security.Claims;

namespace Anet.Web.Jwt;

public class AuthenticateResult
{
    /// <summary>
    /// JWT 验证是否通过
    /// </summary>
    public bool Success { get; set; }
    /// <summary>
    /// 返回消息
    /// </summary>
    public string Message { get; set; }
    /// <summary>
    /// 验证通过后的 Claim 信息
    /// </summary>
    public IList<Claim> Claims { get; set; } = new List<Claim>();
    /// <summary>
    /// 用户信息
    /// </summary>
    public object UserInfo { get; set; }
}

