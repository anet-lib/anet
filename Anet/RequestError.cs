using System.Net;

namespace Anet;

public class RequestError : Exception
{
    public RequestError(string message, HttpStatusCode code = HttpStatusCode.BadRequest) : base(message)
    {
        Code = code;
    }

    public HttpStatusCode Code { get; }

    public static void Throw(string message, HttpStatusCode code = HttpStatusCode.BadRequest)
    {
        throw new RequestError(message, code);
    }

    public static void Throw(bool predicate, string message, HttpStatusCode code = HttpStatusCode.BadRequest)
    {
        if (predicate) throw new RequestError(message, code);
    }

    public static void ThrowUnauthorized(bool predicate = true)
    {
        if (predicate) throw new RequestError("Unauthorized", HttpStatusCode.Unauthorized);
    }

    public static void ThrowForbidden(bool predicate = true)
    {
        if (predicate) throw new RequestError("Forbidden", HttpStatusCode.Forbidden);
    }

    public static void ThrowNotFound(bool predicate = true)
    {
        if (predicate) throw new RequestError("Not Found", HttpStatusCode.NotFound);
    }
}
