using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Anet.Web;

public class ApiResponseAttribute : ActionFilterAttribute
{
    public override void OnActionExecuted(ActionExecutedContext context)
    {
        var result = new ApiResult();
        if (context.Exception != null)
        {
            result.Code = (int)(context.Exception is RequestError err ? err.Code : HttpStatusCode.InternalServerError);
            result.Message = context.Exception.Message;
            context.ExceptionHandled = true;
        }
        else if (context.Result is ObjectResult rst)
        {
            result.Code = rst.StatusCode >= 200 && rst.StatusCode < 300 ? 0 : (rst.StatusCode ?? 0);
            result.Data = rst.Value;
        }
        context.Result = new JsonResult(result);
    }
}
