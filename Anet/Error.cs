namespace Anet;

public enum ErrorCode : ushort
{
    BadRequest = 400,
    Unauthorized = 401,
    Forbidden = 403,
    NotFound = 404,
}

public class Error : Exception
{
    public Error(string message, ErrorCode code = ErrorCode.BadRequest) : base(message)
    {
        Code = code;
    }

    public ErrorCode Code { get; }

    public static void Throw(string message, ErrorCode code = ErrorCode.BadRequest)
    {
        throw new Error(message, code);
    }

    public static void Throw(string message, bool predicate, ErrorCode code = ErrorCode.BadRequest)
    {
        if (predicate) throw new Error(message, code);
    }

    public static void ThrowNotFound(bool predicate = true)
    {
        if (predicate) throw new Error("Not Found", ErrorCode.NotFound);
    }
}
