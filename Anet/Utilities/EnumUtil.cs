using System.Reflection;
using Anet.Atrributes;
using Anet.Models;

namespace Anet.Utilities;

public class EnumUtil
{
    public static IEnumerable<SelectOption> GetSelectOptions(Type enumType)
    {
        foreach (var name in Enum.GetNames(enumType))
        {
            var member = enumType.GetMember(name);
            var display = member[0].GetCustomAttribute<DisplayAttribute>();

            if (display == null || !display.Visible) continue;

            yield return new SelectOption
            {
                Value = Convert.ToInt32(Enum.Parse(enumType, name)),
                Label = display.Name ?? name,
                Order = display.Order,
                Group = display.Group
            };
        }
    }
}
