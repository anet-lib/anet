using System.Net;

namespace Anet.Web;

public class ApiResult<T> : IApiResult
{
    public ApiResult() { }

    public virtual int Code { get; set; }

    public virtual string Message { get; set; }

    public virtual T Data { get; set; }
}

public class ApiResult : ApiResult<object>
{
    public static ApiResult Success(object data = default)
    {
        return new ApiResult { Data = data };
    }

    public static ApiResult Error(string message, HttpStatusCode code = HttpStatusCode.BadRequest)
    {
        return new ApiResult { Message = message, Code = (ushort)code };
    }
}
