namespace Anet.Data;

public static class DbExtensionsEntity
{
    public static Task<T> FindAsync<T>(this Db db, object clause, string table = null)
         where T : new()
    {
        var sql = db.NewSql().Select(table ?? typeof(T).Name, clause);
        return db.SingleOrDefaultAsync<T>(sql, clause);
    }

    public static Task<IEnumerable<T>> QueryAsync<T>(this Db db, object clause, string table = null)
         where T : new()
    {
        var sql = db.NewSql().Select(table ?? typeof(T).Name, clause);
        return db.QueryAsync<T>(sql, clause);
    }

    public static Task InsertAsync<T>(this Db db, T entity, string table = null)
        where T : new()
    {
        var sql = db.NewSql().Insert(table ?? typeof(T).Name, entity);
        return db.ExecuteAsync(sql, entity);
    }

    public static Task InsertBatchAsync<T>(this Db db, IEnumerable<T> entities, string table = null)
    {
        if (entities == null || !entities.Any())
            return Task.CompletedTask;
        var sql = db.NewSql().Insert(table ?? typeof(T).Name, typeof(T));
        return db.ExecuteAsync(sql, entities);
    }

    public static Task<int> UpdateAsync<T>(this Db db, T entity, string keyCols = "Id")
         where T : new()
    {
        return db.UpdateAsync(entity, typeof(T).Name, keyCols);
    }

    public static Task<int> UpdateAsync(this Db db, object entity, string table, string keyCols = "Id")
    {
        var updateCols = SqlString.ParamNames(entity, keyCols);
        var sql = db.NewSql().Update(table, updateCols, keyCols);
        return db.ExecuteAsync(sql, entity);
    }

    public static Task<int> UpdateAsync(this Db db, string table, object update, object clause)
    {
        var sql = db.NewSql().Update(table, update, clause);
        return db.ExecuteAsync(sql, SqlParams.Merge(update, clause));
    }

    public static Task<int> UpdateBatchAsync<T>(this Db db, IEnumerable<T> entities, string keyCols = "Id", string table = null)
         where T : new()
    {
        var updateCols = SqlString.ParamNames(typeof(T), keyCols);
        var sql = db.NewSql().Update(table ?? typeof(T).Name, updateCols, keyCols);
        return db.ExecuteAsync(sql, entities);
    }

    public static Task<int> DeleteAsync<T>(this Db db, object clause)
         where T : new()
    {
        return db.DeleteAsync(typeof(T).Name, clause);
    }

    public static Task<int> DeleteAsync(this Db db, string table, object clause)
    {
        var sql = db.NewSql().Delete(table, clause);
        return db.ExecuteAsync(sql, clause);
    }
}

