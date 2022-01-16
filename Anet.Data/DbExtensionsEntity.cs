namespace Anet.Data;

public static class DbExtensionsEntity
{
    public static Task<T> FindAsync<T>(this Db db, object clause, string table = null)
    {
        var sql = Sql.Select(table ?? typeof(T).Name, clause);
        return db.QuerySingleOrDefaultAsync<T>(sql, clause);
    }

    public static Task<IEnumerable<T>> QueryAsync<T>(this Db db, object clause, string table = null)
    {
        var sql = Sql.Select(table ?? typeof(T).Name, clause);
        return db.QueryAsync<T>(sql, clause);
    }

    public static Task InsertAsync<T>(this Db db, T entity, string table = null)
    {
        var sql = new SqlString().Insert(table ?? typeof(T).Name).Values(entity);
        return db.ExecuteAsync(sql, entity);
    }

    public static Task InsertAsync<T>(this Db db, IEnumerable<T> entities, string table = null)
    {
        if (entities == null || !entities.Any())
            return Task.CompletedTask;
        var sql = Sql.Insert(table ?? typeof(T).Name, typeof(T));
        return db.ExecuteAsync(sql, entities);
    }

    public static Task<int> UpdateAsync<T>(this Db db, T entity, string keyCols = "Id", string table = null)
    {
        var updateCols = Sql.ParamNames(entity, keyCols);
        var sql = Sql.Update(table ?? typeof(T).Name, updateCols, keyCols);
        return db.ExecuteAsync(sql, entity);
    }

    public static Task<int> UpdateAsync<T>(this Db db, IEnumerable<T> entities, string keyCols = "Id", string table = null)
    {
        var updateCols = Sql.ParamNames(typeof(T), keyCols);
        var sql = Sql.Update(table ?? typeof(T).Name, updateCols, keyCols);
        return db.ExecuteAsync(sql, entities);
    }

    public static Task<int> UpdateAsync(this Db db, string table, object entity, string keyCols = "Id")
    {
        var updateCols = Sql.ParamNames(entity, keyCols);
        var sql = Sql.Update(table, updateCols, keyCols);
        return db.ExecuteAsync(sql, entity);
    }

    public static Task<int> UpdateAsync(this Db db, string table, object update, object clause)
    {
        var sql = Sql.Update(table, update, clause);
        return db.ExecuteAsync(sql, SqlParams.Merge(update, clause));
    }

    public static Task<int> DeleteAsync(this Db db, string table, object clause)
    {
        var sql = Sql.Delete(table, clause);
        return db.ExecuteAsync(sql, clause);
    }
}

