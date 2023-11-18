using System.Runtime.CompilerServices;

namespace Anet;

public static class Guard
{
    public static void NotNull(object argument, [CallerArgumentExpression(nameof(argument))] string paramName = null)
    {
        if (argument is null)
        {
            throw new ArgumentNullException(paramName);
        }
    }

    public static void NotNullOrEmpty(string argument, [CallerArgumentExpression(nameof(argument))] string paramName = null)
    {
        NotNull(argument, paramName);
        if (argument == string.Empty)
        {
            throw new ArgumentException("The argument can not be empty.", paramName);
        }
    }

    public static void NotNullOrEmpty<T>(IEnumerable<T> argument, [CallerArgumentExpression(nameof(argument))] string paramName = null)
    {
        NotNull(argument, paramName);
        if (!argument.Any())
        {
            throw new ArgumentException("The collection can not be empty.", paramName);
        }
    }
}
