namespace Anet.Web.Jwt;

public class JwtParams
{
    public string Username { get; set; }

    public string Password { get; set; }

    public string GrantType { get; set; } = GrantTypes.PASSWORD;

    public string RefreshToken { get; set; }

    public Dictionary<string, object> Payload { get; set; }
}
