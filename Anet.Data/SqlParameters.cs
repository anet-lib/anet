using Dapper;

namespace Anet.Data;

public class SqlParameters : DynamicParameters
{
    public SqlParameters() : base()
    {
    }

    public SqlParameters(object template) : base(template)
    {
    }

    public static SqlParameters FromEntity<TEntity>(TEntity entity, Func<TEntity, dynamic> keysSelector)
    {
        return new SqlParameters(keysSelector(entity));
    }
}
