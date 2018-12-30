using System;
using System.Reflection;

namespace Anet.Data
{
    internal static class TypeExtensions
    {
        public static MethodInfo GetPublicInstanceMethod(this Type type, string name, Type[] types)
        {
            return type.GetMethod(name, BindingFlags.Instance | BindingFlags.Public, null, types, null);
        }
    }
}
