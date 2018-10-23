using System.Collections.Generic;
using System.Linq;

namespace Anet
{
    public static class StringExtensions
    {
        public static IEnumerable<string> TrimSplit(this string str, params char[] separator)
        {
            return str.Split(separator).Select(s => s.Trim());
        }
    }
}
