using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Anet
{
    public static class EnumExtenstions
    {
        public static string GetDisplayName(this Enum value)
        {
            var type = value.GetType();
            var name = Enum.GetName(type, value);
            var attribute = type.GetField(name).GetCustomAttribute<DisplayAttribute>();
            return attribute == null ? name : attribute.Name;
        }
    }
}
