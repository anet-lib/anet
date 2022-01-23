namespace System;

public static class TypeExtensions
{
    /// <summary>
    /// Returns true if the type is one of the built in simple types.
    /// </summary>
    public static bool IsSimpleType(this Type type)
    {
        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            type = type.GetGenericArguments()[0]; // or  Nullable.GetUnderlyingType(type)

        if (type.IsEnum)
            return true;

        if (type == typeof(Guid))
            return true;

        TypeCode tc = Type.GetTypeCode(type);
        return tc switch
        {
            TypeCode.Byte or 
            TypeCode.SByte or 
            TypeCode.Int16 or 
            TypeCode.Int32 or 
            TypeCode.Int64 or 
            TypeCode.UInt16 or 
            TypeCode.UInt32 or 
            TypeCode.UInt64 or 
            TypeCode.Single or 
            TypeCode.Double or 
            TypeCode.Decimal or 
            TypeCode.Char or 
            TypeCode.String or 
            TypeCode.Boolean or 
            TypeCode.DateTime => true,
            TypeCode.Object => (typeof(TimeSpan) == type) || (typeof(DateTimeOffset) == type),
            _ => false,
        };
    }

    public static Type GetElementType(this Type type)
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
