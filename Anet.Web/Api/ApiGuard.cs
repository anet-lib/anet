namespace Anet.Web.Api;

public static class ApiGuard
{
    public static void EnsureNotExist(object value, string message = "Already existed")
    {
        if (value != null) throw new BadRequestException(message);
    }

    public static void EnsureExist(object value, string message = "Not found")
    {
        if (value == null) throw new BadRequestException(message);
    }

    public static void EnsureNotEmpty<T>(IEnumerable<T> list, string message = "Parameters is empty")
    {
        if (list == null || !list.Any())
            throw new BadRequestException(message);
    }

    public static void EnsureNotDuplicated<T>(IEnumerable<T> list, string message = "Parameters is empty")
    {
        if (list == null || !list.Any())
            return;
        if (list.Distinct().Count() < list.Count())
        {
            throw new BadRequestException(message);
        }
    }

    public static void BadRequest(string message)
    {
        throw new BadRequestException(message);
    }
}
