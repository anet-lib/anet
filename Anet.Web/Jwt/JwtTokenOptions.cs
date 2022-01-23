namespace Anet.Web.Jwt;

/// <summary>
/// Provides options for <see cref="JwtProvider"/>.
/// </summary>
public class JwtTokenOptions
{
    public string Path { get; set; } = "/token";

    /// <summary>
    /// The key for signature validation.
    /// </summary>
    public string SigningKey { get; set; }

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
    /// </summary>
    /// <remarks>The default is 60 minutes (3600 seconds).</remarks>
    public int Expiration { get; set; } = (int)TimeSpan.FromMinutes(60).TotalSeconds;

    /// <summary>
    /// The fallback cookie key that read token from.
    /// Default value is "Authorization".
    /// </summary>
    public string FallbackCookieKey { get; set; } = "Authorization";
}
