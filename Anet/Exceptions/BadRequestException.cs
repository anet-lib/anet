namespace Anet;

public class BadRequestException : Exception
{
    public BadRequestException(string message = null) : base(message) { }

    public static void ThrowIf(bool predicate, string message = null)
    {
        if (predicate)
            throw new BadRequestException(message);
    }
}
