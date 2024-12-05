using System.Net;

namespace Anet;

public class Error(string message, HttpStatusCode code = HttpStatusCode.BadRequest) : Exception(message)
{
    public HttpStatusCode Code { get; } = code;

    public static void Throw(string message, HttpStatusCode code = HttpStatusCode.BadRequest)
    {
        throw new Error(message, code);
    }

    public static void Throw(bool predicate, string message, HttpStatusCode code = HttpStatusCode.BadRequest)
    {
        if (predicate) throw new Error(message, code);
    }

    public static void ThrowUnauthorized(bool predicate = true, string message= "Unauthorized")
    {
        if (predicate) throw new Error(message, HttpStatusCode.Unauthorized);
    }

    public static void ThrowForbidden(bool predicate = true, string message = "Forbidden")
    {
        if (predicate) throw new Error(message, HttpStatusCode.Forbidden);
    }

    public static void ThrowNotFound(bool predicate = true, string message = "Not Found")
    {
        if (predicate) throw new Error(message, HttpStatusCode.NotFound);
    }
}
