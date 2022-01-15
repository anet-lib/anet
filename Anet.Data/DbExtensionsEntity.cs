using System.Data;

namespace Anet.Data;

public static class DbExtensionsEntity
{
    public static Task<TEntity> FindAsync<TEntity>(this Db db, object clause, string table = null)
    {
        var sql = Sql.Select(table ?? typeof(TEntity).Name, clause);
        return db.QuerySingleOrDefaultAsync<TEntity>(sql, clause);
    }

    public static Task<IEnumerable<TEntity>> QueryAsync<TEntity>(this Db db, object clause, string table = null)
    {
        var sql = Sql.Select(table ?? typeof(TEntity).Name, clause);
        return db.QueryAsync<TEntity>(sql, clause);
    }

    public static Task InsertAsync<TEntity>(this Db db, TEntity entity, string table = null)
    {
        var sql = Sql.Insert(table ?? typeof(TEntity).Name, entity);
        return db.ExecuteAsync(sql, entity);
    }

    public static Task InsertAsync<TEntity>(this Db db, IEnumerable<TEntity> entities, string table = null)
    {
        if (entities == null || !entities.Any())
            return Task.CompletedTask;
        var sql = Sql.Insert(table ?? typeof(TEntity).Name, typeof(TEntity));
        return db.ExecuteAsync(sql, entities);
    }

    public static Task<int> UpdateAsync<TEntity>(this Db db, TEntity entity, string table = null, string keyCols = "Id")
    {
        var updateColumns = Sql.GetParamNames(entity).Where(x => keyCols.Contains(x, StringComparison.OrdinalIgnoreCase));
        var sql = Sql.Update(table ?? typeof(TEntity).Name, updateColumns, keyCols);
        return db.ExecuteAsync(sql, entity);
    }

    public static Task<int> UpdateAsync<TEntity>(this Db db, IEnumerable<TEntity> entities, string table = default, string keyCols = "Id")
    {
        var updateColumns = Sql.GetParamNames(typeof(TEntity)).Where(x => keyCols.Contains(x, StringComparison.OrdinalIgnoreCase));
        var sql = Sql.Update(table ?? typeof(TEntity).Name, updateColumns, keyCols);
        return db.ExecuteAsync(sql, entities);
    }

    public static Task<int> UpdateAsync(this Db db, object update, object clause, string table = null)
    {
        var sql = Sql.Update(table, update, clause);
        return db.ExecuteAsync(sql, Sql.MergeParams(update, clause));
    }

    public static Task<int> DeleteAsync(this Db db, object clause, string table = null)
    {
        var sql = Sql.Delete(table, clause);
        return db.ExecuteAsync(sql, clause);
    }
}

