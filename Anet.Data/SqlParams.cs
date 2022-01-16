using Dapper;

namespace Anet.Data;

public class SqlParams : DynamicParameters
{
    /// <summary>
    /// Construct a dynamic parameter bag
    /// </summary>
    public SqlParams() : base()
    {
    }
    /// <summary>
    /// Construct a dynamic parameter bag
    /// </summary>
    /// <param name="template">Can be an anonymous type, a SqlParams or a DynamicParameters bag</param>
    public SqlParams(object template) : base(template)
    {
    }

    public string Sql { get; private set; }

    public static SqlParams From<T>(T obj, Func<T, dynamic> keysSelector)
    {
        return new SqlParams(keysSelector(obj));
    }

    /// <summary>
    /// Merge parameters.
    /// </summary>
    /// <param name="firstParam">The first parameter to merge.</param>
    /// <param name="otherParams">The other prameters to merge.</param>
    /// <returns>Merged parameters.</returns>
    public static SqlParams Merge(object firstParam, params object[] otherParams)
    {
        Guard.NotNull(firstParam, nameof(firstParam));

        var parameters = new SqlParams();
        parameters.AddDynamicParams(firstParam);

        foreach (var param in otherParams)
        {
            parameters.AddDynamicParams(param);
        }

        return parameters;
    }
}
