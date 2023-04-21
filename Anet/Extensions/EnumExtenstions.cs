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

    public static IEnumerable<SelectOption> GetSelectOptions<TEnum>()
        where TEnum : struct, Enum
    {
        var type = typeof(TEnum);
        foreach (var name in Enum.GetNames<TEnum>())
        {
            var member = type.GetMember(name);
            var display = member[0].GetCustomAttribute<DisplayAttribute>();

            if (display == null || !display.Visible) continue;

            yield return new SelectOption()
            {
                Value = Convert.ToInt32(Enum.Parse<TEnum>(name)),
                Name = name,
                Label = display.Name ?? name,
                Order = display.Order,
                Group = display.Group
            };
        }
    }

    public static IEnumerable<SelectOption<string>> GetSelectOptionsNameAsValue<TEnum>()
       where TEnum : struct, Enum
    {
        var type = typeof(TEnum);
        foreach (var name in Enum.GetNames<TEnum>())
        {
            var member = type.GetMember(name);
            var display = member[0].GetCustomAttribute<DisplayAttribute>();

            if (display == null || !display.Visible) continue;

            yield return new SelectOption<string>()
            {
                Value = name,
                Name = name,
                Label = display.Name ?? name,
                Order = display.Order,
                Group = display.Group
            };
        }
    }
}
