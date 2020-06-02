using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace System
{
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
            return attribute == null ? value.ToString() : attribute.Name;
        }

        public static string GetDisplayDescription(this Enum value)
        {
            var attribute = value.GetCustomAttribute<DisplayAttribute>();
            return attribute == null ? value.ToString() : attribute.Description;
        }

        public static string GetDescription(this Enum value)
        {
            var attribute = value.GetCustomAttribute<DescriptionAttribute>();
            return attribute == null ? value.ToString() : attribute.Description;
        }
    }
}
