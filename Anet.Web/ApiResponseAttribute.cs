using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Anet.Web;

public class ApiResponseAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        if (context.ModelState.IsValid) return;

        var errorMessage = "The request data is invalid.";

        foreach (var key in context.ModelState.Keys)
        {
            var value = context.ModelState[key];
            if (value.Errors != null && value.Errors.Count > 0)
            {
                var message = value.Errors[0].ErrorMessage;
                errorMessage = string.IsNullOrEmpty(message)
                    ? $"The value of {key} field is invalid." : message;
                break;
            }
        }

        context.Result = new JsonResult(ApiResult.Error(errorMessage));
    }

    public override void OnActionExecuted(ActionExecutedContext context)
    {
        if (context.Result is FileResult ||
            context.HttpContext.WebSockets.IsWebSocketRequest)
        {
            return;
        }

        var result = new ApiResult();
        if (context.Exception != null)
        {
            result.Code = (int)(context.Exception is Error err ? err.Code : HttpStatusCode.InternalServerError);
            result.Message = context.Exception.Message;
            context.ExceptionHandled = result.Code < 500;
        }
        else if (context.Result is ObjectResult rst)
        {
            result.Code = rst.StatusCode >= 200 && rst.StatusCode < 300 ? 0 : (rst.StatusCode ?? 0);
            result.Data = rst.Value;
        }

        context.Result = new JsonResult(result);
    }
}
