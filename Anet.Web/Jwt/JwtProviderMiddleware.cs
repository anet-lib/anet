using Anet.Utilities;
using Anet.Web.Api;
using Microsoft.AspNetCore.Http;
using System.Net;
using System.Text;

namespace Anet.Web.Jwt;

internal class JwtProviderMiddleware
{
    private readonly RequestDelegate _next;
    private readonly JwtOptions _options;
    private readonly JwtProvider _provider;
    private HttpContext _context;
    private IAuthenticator _authenticator;
    private IRefreshTokenStore _refreshTokenStore;

    public JwtProviderMiddleware(
        RequestDelegate next, JwtOptions options, JwtProvider provider)
    {
        _next = next;
        _options = options;
        _provider = provider;
    }

    public async Task InvokeAsync(
        HttpContext context,
        IAuthenticator authenticator,
        IRefreshTokenStore refreshTokenStore)
    {
        var request = context.Request;
        if (request.Path != _options.Path || request.Method != HttpMethods.Post)
        {
            await _next(context);
            return;
        }

        _context = context;
        _authenticator = authenticator;
        _refreshTokenStore = refreshTokenStore;

        var requestParams = await ResolveTokenRequest(request);
        if (requestParams == null) return;

        if (GrantTypes.REFRESH_TOKEN.Equals(requestParams.GrantType, StringComparison.OrdinalIgnoreCase))
        {
            await RefreshToken(requestParams);
        }
        else
        {
            await GenerateToken(requestParams);
        }
    }

    private async Task<JwtParams> ResolveTokenRequest(HttpRequest request)
    {
        var contentType = request.ContentType?.ToLower();
        if (contentType.Contains(ContentTypes.FormUrlencodedContentType))
        {
            return new JwtParams
            {
                Username = request.Form[nameof(JwtParams.Username)].FirstOrDefault(),
                Password = request.Form[nameof(JwtParams.Password)].FirstOrDefault(),
                Payload = request.Form[nameof(JwtParams.Payload)].FirstOrDefault(),
                GrantType = request.Form[nameof(JwtParams.GrantType)].FirstOrDefault(),
                RefreshToken = request.Form[nameof(JwtParams.RefreshToken)].FirstOrDefault()
            };
        }
        if (contentType.Contains(ContentTypes.JsonContentType))
        {
            using var reader = new StreamReader(request.Body, Encoding.UTF8);
            string body = await reader.ReadToEndAsync();
            try
            {
                return JsonUtility.DeserializeCamelCase<JwtParams>(body);
            }
            catch
            {
                await ResponseErrorAsync("Unrecognized request body.");
                return null;
            }
        }
        await ResponseErrorAsync("Unsurported Content-Type.");
        return null;
    }

    private async Task GenerateToken(JwtParams jwtParams)
    {
        var authResult = await _authenticator.AuthenticateAsync(jwtParams);

        if (authResult == null || !authResult.Success)
        {
            await ResponseErrorAsync(authResult?.Message);
            return;
        }

        var jwtResult = _provider.GenerateToken(authResult.Claims);

        await _refreshTokenStore.SaveTokenAsync(jwtResult);

        jwtResult.UserInfo = authResult.UserInfo;

        await ResponseSuccessAsync(jwtResult);
    }

    private async Task RefreshToken(JwtParams jwtParams)
    {
        var newToken = _provider.RefreshToken(jwtParams.RefreshToken);
        if (newToken == null)
        {
            await ResponseErrorAsync("Invalid refresh token.");
            return;
        }
        await ResponseSuccessAsync(newToken);
    }

    private Task ResponseSuccessAsync<T>(T data)
    {
        return ResponseAsync(ApiResult.Success(data));
    }

    private Task ResponseErrorAsync(string message)
    {
        return ResponseAsync(ApiResult.Error(message));
    }

    private Task ResponseAsync(ApiResult apiResult)
    {
        _context.Response.ContentType = ContentTypes.JsonContentType;
        _context.Response.StatusCode = (int)HttpStatusCode.OK;
        var json = JsonUtility.SerializeCamelCase(apiResult);
        return _context.Response.WriteAsync(json);
    }
}
