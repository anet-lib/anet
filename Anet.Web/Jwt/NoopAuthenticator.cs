namespace Anet.Web.Jwt;

public class NoopAuthenticator : IAuthenticator
{
    public Task<AuthenticateResult> AuthenticateAsync(JwtParams jwtParams)
    {
        throw new NotImplementedException();
    }
}

