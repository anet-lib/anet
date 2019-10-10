using System.Collections.Generic;

namespace System.Linq
{
    public static class LinqExtensions
    {
        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            HashSet<TKey> knownKeys = new HashSet<TKey>();

            foreach (TSource element in source)
            {
                if (knownKeys.Add(keySelector(element)))
                {
                    yield return element;
                }
            }
        }

        public static IQueryable<T> Paged<T>(this IQueryable<T> query, int page, int size)
        {
            return query.Skip((page - 1) * size).Take(size);
        }
    }
}
