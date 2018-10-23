using System;
using System.Reflection;

namespace Anet
{
    public static class TypeExtensions
    {
        private static bool IsSimpleType(this Type type)
        {
            var typeInfo = type.GetTypeInfo();

            return typeInfo.IsPrimitive ||
                   typeInfo.IsEnum ||
                   type.Equals(typeof(string)) ||
                   type.Equals(typeof(DateTime)) ||
                   type.Equals(typeof(Decimal)) ||
                   type.Equals(typeof(Guid)) ||
                   type.Equals(typeof(DateTimeOffset)) ||
                   type.Equals(typeof(TimeSpan));
        }

        private static bool IsSimpleUnderlyingType(this Type type)
        {
            Type underlyingType = Nullable.GetUnderlyingType(type);

            if (underlyingType != null)
            {
                type = underlyingType;
            }

            return IsSimpleType(type);
        }
    }
}
