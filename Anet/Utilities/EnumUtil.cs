using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Anet;

public class EnumDisplay
{
    public int Value { get; set; }
    public string Field { get; set; }
    public string Name { get; set; }
    public int Order { get; set; }
    public string Group { get; set; }
    public bool Checked { get; set; }
}

public class EnumUtil
{
    public static IEnumerable<EnumDisplay> GetDisplayList(Type enumType)
    {
        foreach (var name in Enum.GetNames(enumType))
        {
            var member = enumType.GetMember(name);
            // 忽略过时枚举值
            var obsoleteAttribute = member[0].GetCustomAttribute<ObsoleteAttribute>();
            if (obsoleteAttribute != null)
                continue;
            // 忽略没有 DisplayAttribute 的枚举值
            var displayAttribute = member[0].GetCustomAttribute<DisplayAttribute>();
            if (displayAttribute == null)
                continue;

            yield return new EnumDisplay
            {
                Value = Convert.ToInt32(Enum.Parse(enumType, name)),
                Field = name,
                Name = displayAttribute.GetName(),
                Order = displayAttribute.GetOrder() ?? 0,
                Group = displayAttribute.GetGroupName()
            };
        }
    }
}
