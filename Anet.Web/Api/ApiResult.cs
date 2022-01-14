namespace Anet.Web.Api;

public class ApiResult<T> : IApiResult
{
    public ApiResult() { }

    public virtual short Code { get; set; }

    public virtual string Message { get; set; }

    public virtual T Data { get; set; }
}

public class ApiResult : ApiResult<object>
{
    public const short DefaultSuccessCode = 0;
    public const short DefaultErrorCode = 400;

    public static ApiResult Success(object data = default, short code = DefaultSuccessCode)
    {
        return new ApiResult { Data = data, Code = code };
    }

    public static ApiResult Error(string message, short code = DefaultErrorCode)
    {
        return new ApiResult { Message = message, Code = code };
    }
}
