using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace Anet.Web
{
    public class ExceptionHandlerFilterAttribute : ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext context)
        {
            var exception = context.Exception;
            if (exception == null)
                return; // Should never happen.

            var result = new ObjectResult("An error has occurred.");

            if (exception is BadRequestException badRequest)
            {
                result.StatusCode = StatusCodes.Status400BadRequest;
                result.Value = badRequest.Message;
            }
            else if (exception is NotFoundException)
            {
                result.StatusCode = StatusCodes.Status404NotFound;
                result.Value = "Resource not found.";
            }
            else if (exception is UnauthorizedAccessException)
            {
                result.StatusCode = StatusCodes.Status401Unauthorized;
                result.Value = "Unauthorized request.";
            }
            //else if (exception is SecurityTokenValidationException)
            //{
            //    result.StatusCode = StatusCodes.Status403Forbidden;
            //    result.Value = "Invalid token.";
            //}
            else
            {
                result.StatusCode = StatusCodes.Status500InternalServerError;
                result.Value = "An unhandled server error has occurred.";

                var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<ExceptionHandlerFilterAttribute>>();
                logger.LogError(0, exception, exception.Message);

                var env = context.HttpContext.RequestServices.GetRequiredService<IHostingEnvironment>();
                if (env.IsDevelopment())
                {
                    throw exception;
                }
            }

            context.Result = result;
        }
    }
}
