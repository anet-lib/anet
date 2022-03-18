using System.Reflection;
using Anet.Atrributes;

namespace System;

public static class EnumExtenstions
{
    public static T GetCustomAttribute<T>(this Enum value) where T : Attribute
    {
        var field = value.GetType().GetField(value.ToString());
        return field.GetCustomAttribute<T>();
    }

    public static string GetDisplayName(this Enum value)
    {
        var attribute = value.GetCustomAttribute<DisplayAttribute>();
        return attribute == null ? value.ToString() : attribute.Label;
    }

    public static string GetDisplayDescription(this Enum value)
    {
        var attribute = value.GetCustomAttribute<DisplayAttribute>();
        return attribute == null ? value.ToString() : attribute.Description;
    }
}
