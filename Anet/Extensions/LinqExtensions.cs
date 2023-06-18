namespace System.Linq;

public static class LinqExtensions
{
    public static IQueryable<T> Paged<T>(this IQueryable<T> query, int page, int size)
    {
        return query.Skip((page - 1) * size).Take(size);
    }
}
