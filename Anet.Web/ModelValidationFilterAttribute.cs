using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Anet.Web;

public class ModelValidationFilterAttribute : ActionFilterAttribute
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

        context.Result = new JsonResult(ApiResult.Error(errorMessage, HttpStatusCode.BadRequest));
    }
}
