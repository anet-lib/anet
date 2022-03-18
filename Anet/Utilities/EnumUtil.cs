using System.Reflection;
using Anet.Atrributes;
using Anet.Models;

namespace Anet.Utilities;

public class EnumUtil
{
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
                Label = display.Label ?? name,
                Order = display.Order,
                Group = display.Group
            };
        }
    }
}
