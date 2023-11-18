using System.Numerics;

namespace System;
#pragma warning disable IDE0049 // Simplify Names
public static class TypeExtensions
{
    public static bool IsInteger(this ValueType value)
    {
        return value is SByte or Int16 or Int32 or Int64 or Byte or UInt16 or UInt32 or UInt64 or BigInteger;
    }

    public static bool IsFloat(this ValueType value)
    {
        return value is Single or Double or Decimal;
    }

    public static bool IsNumeric(this ValueType value)
    {
        return IsInteger(value) || IsFloat(value);
    }

    /// <summary>
    /// Returns true if the type is one of the built in simple types.
    /// </summary>
    public static bool IsSimpleType(this Type type)
    {
        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            type = type.GetGenericArguments()[0]; // or Nullable.GetUnderlyingType(type)

        // The primitive types are Boolean, Byte, SByte, Int16, UInt16, Int32, UInt32, Int64, UInt64, IntPtr, UIntPtr, Char, Double, and Single.
        if (type.IsPrimitive || type.IsEnum || type == typeof(Guid))
            return true;

        TypeCode tc = Type.GetTypeCode(type);
        return tc switch
        {
            TypeCode.Decimal or
            TypeCode.String or
            TypeCode.DateTime => true,
            TypeCode.Object => (typeof(TimeSpan) == type) || (typeof(DateTimeOffset) == type) || (typeof(DateOnly) == type) || (typeof(TimeOnly) == type),
            _ => false,
        };
    }

    public static Type GetAnyElementType(this Type type)
    {
        // short-circuit if you expect lots of arrays 
        if (type.IsArray)
            return type.GetElementType();

        // type is IEnumerable<T>;
        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>))
            return type.GetGenericArguments()[0];

        // type implements/extends IEnumerable<T>;
        var enumType = type.GetInterfaces()
            .Where(t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IEnumerable<>))
            .Select(t => t.GenericTypeArguments[0]).FirstOrDefault();

        return enumType ?? type;
    }
}
#pragma warning restore IDE0049 // Simplify Names
