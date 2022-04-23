namespace Anet.Web.Jwt;

/// <summary>
/// Provides options for <see cref="JwtProvider"/>.
/// </summary>
public class JwtTokenOptions
{
    /// <summary>
    /// The key for signature validation.
    /// </summary>
    public string Key { get; set; }

    /// <summary>
    ///  The Issuer (iss) claim for generated tokens.
    /// </summary>
    public string Issuer { get; set; }

    /// <summary>
    /// The Audience (aud) claim for the generated tokens.
    /// </summary>
    public string Audience { get; set; }

    /// <summary>
    /// The expiration time for the generated tokens.
    /// Set to 0 to never expire.
    /// </summary>
    public int ExpireSeconds { get; set; }

    /// <summary>
    /// The fallback cookie key that read token from.
    /// This will give the application an opportunity to retrieve a token from cookies as an alternative location.
    /// </summary>
    public string FallbackCookieKey { get; set; } = "Authorization";
}
