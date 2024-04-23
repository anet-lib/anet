using System.Reflection;
using Anet.Atrributes;
using Anet.Models;

namespace Anet.Utilities;

public class EnumUtil
{
    public static IEnumerable<SelectOption<int>> GetSelectOptions<TEnum>(bool includeVisible = false)
     where TEnum : struct, Enum
    {
        return GetSelectOptions<TEnum, int>(includeVisible);
    }

    public static IEnumerable<SelectOption<TValue>> GetSelectOptions<TEnum, TValue>(bool includeVisible = false)
        where TEnum : struct, Enum
        where TValue : IEquatable<TValue>
    {
        var type = typeof(TEnum);
        foreach (var name in Enum.GetNames<TEnum>())
        {
            var member = type.GetMember(name);
            var display = member[0].GetCustomAttribute<DisplayAttribute>();

            if (display == null || (!display.Visible && !includeVisible)) continue;

            yield return new SelectOption<TValue>()
            {
                Value = (TValue)Enum.Parse(type, name),
                Name = name,
                Label = display.Name ?? name,
                Order = display.Order,
                Group = display.Group
            };
        }
    }
}
