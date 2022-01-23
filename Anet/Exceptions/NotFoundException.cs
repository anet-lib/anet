namespace Anet;

public class NotFoundException : Exception
{
    public NotFoundException(string message = null) : base(message)
    {
    }

    public static void ThrowIf(bool predicate, string message = null)
    {
        if (predicate)
            throw new BadRequestException(message);
    }
}
