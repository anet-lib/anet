using System;
using System.Collections.Generic;
using System.Linq;

namespace Anet
{
    public static class Ensure
    {
        public static void NotNull(object param, string paramName)
        {
            if (param == null)
                throw new ArgumentNullException(paramName);
        }

        public static void NotNullOrEmpty(string paramValue,string paramName)
        {
            if(string.IsNullOrEmpty(paramValue))
                throw new ArgumentNullException(paramName);
        }

        public static void HasItems<T>(IEnumerable<T> collection, string paramName)
        {
            if (collection == null)
                throw new ArgumentNullException(paramName);
            if (collection.Count() == 0)
                throw new ArgumentException("Collection must have items.", paramName);
        }
    }
}
