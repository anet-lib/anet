using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Anet.Web
{
    internal class ModelStateValidationFilterAttribute : ActionFilterAttribute
    {
        public override void OnResultExecuting(ResultExecutingContext context)
        {
            if (context.ModelState.IsValid) return;

            var errorMessage = "The request data is invalid.";

            foreach (var key in context.ModelState.Keys)
            {
                var value = context.ModelState[key];
                if (value.Errors != null && value.Errors.Count > 0)
                {
                    errorMessage = key + ": " + value.Errors[0].ErrorMessage;
                    break;
                }
            }
            context.Result = new BadRequestObjectResult(errorMessage);
        }
    }
}
