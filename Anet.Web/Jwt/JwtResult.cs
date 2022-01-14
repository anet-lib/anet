namespace Anet.Web.Jwt;

public class JwtResult
{
    public string AccessToken { get; set; }

    public long ExpiresAt { get; set; }

    public string RefreshToken { get; set; }

    public object UserInfo { get; set; }
}

