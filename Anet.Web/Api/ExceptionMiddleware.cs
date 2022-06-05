using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Anet.Web.Api;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;
    private readonly IEnumerable<string> _pathPrefixes;

    public ExceptionMiddleware(
        RequestDelegate next,
        ILogger<ExceptionMiddleware> logger,
        IEnumerable<string> pathPrefixes = null)
    {
        _next = next;
        _logger = logger;
        _pathPrefixes = pathPrefixes;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        if (_pathPrefixes == null ||
            !_pathPrefixes.Any() ||
            _pathPrefixes.Any(x => httpContext.Request.Path.StartsWithSegments(x)))
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(httpContext, ex);
            }
        }
        else
        {
            await _next(httpContext);
        }
    }

    private Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var result = new ApiResult
        {
            Message = exception.Message,
            Code = 500
        };

        if (exception is Error err)
        {
            result.Code = (ushort)err.Code;
        }

        if (result.Code >= 500)
        {
            _logger.LogError(exception, exception.Message);
        }

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)HttpStatusCode.OK;

        return context.Response.WriteAsJsonAsync(result);
    }
}
