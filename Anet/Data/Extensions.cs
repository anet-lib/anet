using System.Data.Common;

namespace Anet.Data;

internal static class Extensions
{
    /// <summary>
    /// Extracts the parameter name-value pairs from a DBCommand
    /// </summary>
    /// <param name="command">The DBCommand</param>
    /// <param name="hideValues">Replace values with a mask</param>
    /// <returns>Parameter values by names</returns>
    public static Dictionary<string, object> GetParameters(this DbCommand command, bool hideValues = false)
    {
        IEnumerable<DbParameter> GetParameters()
        {
            foreach (DbParameter parameter in command.Parameters)
                yield return parameter;
        }

        return GetParameters().ToDictionary(
            k => k.ParameterName,
            v => hideValues ? "?" : v.Value == null || v.Value is DBNull ? "<null>" : v.Value);
    }
}

