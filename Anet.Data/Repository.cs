using System.Data;

namespace Anet.Data;

/// <summary>
/// A base class for a repository.
/// </summary>
/// <typeparam name="TEntity">The type of entity.</typeparam>
/// <typeparam name="TKey">The type of primary key.</typeparam>
public class Repository<TEntity, TKey> : IRepository<TEntity, TKey> 
    where TEntity : class
    where TKey : IEquatable<TKey>
{
    /// <summary>
    /// Initialize the base class of a repository.
    /// </summary>
    /// <param name="db">The database to access.</param>
    public Repository(Db db)
    {
        Db = db;
    }

    /// <summary>
    /// The database to access.
    /// </summary>
    public Db Db { get; }

    public string TableName { get => typeof(TEntity).Name; }

    /// <summary>
    /// Begins a database transaction.
    /// </summary>
    /// <returns>An object representing the new transaction.</returns>
    public IDbTransaction BeginTransaction()
    {
        return BeginTransaction(IsolationLevel.Unspecified);
    }

    /// <summary>
    /// Begins a database transaction with the specified <see cref="IsolationLevel"/> value.
    /// </summary>
    /// <param name="il">One of the <see cref="IsolationLevel"/> values.</param>
    /// <returns> An object representing the new transaction.</returns>
    public IDbTransaction BeginTransaction(IsolationLevel il)
    {
        return Db.BeginTransaction(il);
    }

    #region CRUD Mehthods

    public virtual Task<TEntity> FindAsync(TKey id)
    {
        return FindAsync(new { Id = id });
    }

    public virtual Task<TEntity> FindAsync(object clause)
    {
        var sql = Sql.Select(TableName, clause);
        return Db.QuerySingleOrDefaultAsync<TEntity>(sql, clause);
    }

    public virtual Task<IEnumerable<TEntity>> QueryAsync(object clause)
    {
        var sql = Sql.Select(TableName, clause);
        return Db.QueryAsync<TEntity>(sql, clause);
    }

    public virtual Task InsertAsync(TEntity entity)
    {
        var sql = Sql.Insert(TableName, entity);
        return Db.ExecuteAsync(sql, entity);
    }

    public virtual Task InsertAsync(IEnumerable<TEntity> entities)
    {
        if (entities == null || !entities.Any())
            return Task.CompletedTask;
        var sql = Sql.Insert(TableName, typeof(TEntity));
        return Db.ExecuteAsync(sql, entities);
    }

    public virtual Task<int> UpdateAsync(TEntity entity, string primaryKey = "Id")
    {
        var updateColumns = Sql.GetParamNames(entity).Where(x => x != primaryKey);
        var sql = Sql.Update(TableName, updateColumns, primaryKey);
        return Db.ExecuteAsync(sql, entity);
    }

    public virtual Task<int> UpdateAsync(IEnumerable<TEntity> entities, string primaryKey = "Id")
    {
        var updateColumns = Sql.GetParamNames(typeof(TEntity)).Where(x => x != primaryKey);
        var sql = Sql.Update(TableName, updateColumns, primaryKey);
        return Db.ExecuteAsync(sql, entities);
    }

    public virtual Task<int> UpdateAsync(object update, object clause)
    {
        var sql = Sql.Update(TableName, update, clause);
        return Db.ExecuteAsync(sql, Sql.MergeParams(update, clause));
    }

    public virtual Task<int> DeleteAsync(TKey id)
    {
        return DeleteAsync(new { Id = id });
    }

    public virtual Task<int> DeleteAsync(object clause)
    {
        var sql = Sql.Delete(TableName, clause);
        return Db.ExecuteAsync(sql, clause);
    }

    #endregion
}

/// <summary>
/// A base class for a repository.
/// </summary>
/// <typeparam name="TEntity">The type of entity.</typeparam>
public class Repository<TEntity> : Repository<TEntity, long> where TEntity : class
{
    public Repository(Db db) : base(db)
    {
    }
}
