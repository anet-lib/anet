using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Anet.Web.Api;

public class ApiResultFilterAttribute : ResultFilterAttribute
{
    public override void OnResultExecuting(ResultExecutingContext context)
    {
        if (context.Result is EmptyResult)
        {
            context.Result = new ObjectResult(ApiResult.Success());
        }
        else if (context.Result is ObjectResult result)
        {
            context.Result = new ObjectResult(ApiResult.Success(result.Value));
        }
        base.OnResultExecuting(context);
    }
}

