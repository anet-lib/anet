using System.Reflection;
using Anet.Atrributes;
using Anet.Models;

namespace System;

public static class EnumEx
{
    public static T GetCustomAttribute<T>(this Enum value) where T : Attribute
    {
        var field = value.GetType().GetField(value.ToString());
        return field == null ? default : field.GetCustomAttribute<T>();
    }

    public static string GetDisplayName(this Enum value)
    {
        var attribute = value.GetCustomAttribute<DisplayAttribute>();
        return attribute == null ? value.ToString() : attribute.Name;
    }

    public static string GetDisplayDescription(this Enum value)
    {
        var attribute = value.GetCustomAttribute<DisplayAttribute>();
        return attribute == null ? value.ToString() : attribute.Description;
    }
}
