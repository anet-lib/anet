using System.Net;
using System.Text;
using Anet.Utilities;
using Microsoft.AspNetCore.Http;

namespace Anet.Web.Jwt;

[Obsolete("已废弃")]
internal class JwtProviderMiddleware
{
    private readonly string _path;
    private readonly RequestDelegate _next;
    private JwtProvider _provider;
    private HttpContext _context;
    private IAuthenticator _authenticator;

    public JwtProviderMiddleware(
        RequestDelegate next, string tokenPath)
    {
        _next = next;
        _path = tokenPath;
    }

    public async Task InvokeAsync(
        HttpContext context,
        JwtProvider provider,
        IAuthenticator authenticator)
    {
        var request = context.Request;
        if (!request.Path.Equals(_path, StringComparison.OrdinalIgnoreCase) || request.Method != HttpMethods.Post)
        {
            await _next(context);
            return;
        }

        _context = context;
        _provider = provider;
        _authenticator = authenticator;

        var requestParams = await ResolveTokenRequest(request);
        if (requestParams == null) return;

        if (GrantTypes.REFRESH_TOKEN.Equals(requestParams.GrantType, true))
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
                // Payload = request.Form[nameof(JwtParams.Payload)].FirstOrDefault(),
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
                return JsonUtil.DeserializeCamelCase<JwtParams>(body);
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

        var jwtResult = await _provider.GenerateToken(authResult.Claims);

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
        return ResponseAsync(ApiResult.Error(message, HttpStatusCode.BadRequest));
    }

    private Task ResponseAsync(ApiResult apiResult)
    {
        _context.Response.ContentType = ContentTypes.JsonContentType;
        _context.Response.StatusCode = (int)HttpStatusCode.OK;
        var json = JsonUtil.SerializeCamelCase(apiResult);
        return _context.Response.WriteAsync(json);
    }
}
