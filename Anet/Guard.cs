using System;
using System.Collections.Generic;
using System.Linq;

namespace Anet
{
    public static class Guard
    {
        public static void NotNull(object param, string paramName)
        {
            if (param is null)
            {
                throw new ArgumentNullException(paramName);
            }
        }

        public static void NotNullOrEmpty(string param, string paramName)
        {
            NotNull(param, paramName);
            if (param == string.Empty)
            {
                throw new ArgumentException($"The string can not be empty.", paramName);
            }
        }

        public static void NotNullOrEmpty<T>(IEnumerable<T> param, string paramName)
        {
            NotNull(param, paramName);
            if (param.Count() == 0)
            {
                throw new ArgumentException("The collection can not be empty.", paramName);
            }
        }
    }
}
