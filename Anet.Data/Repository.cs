using Anet.Entity;
using Dapper;
using System.Data;

namespace Anet.Data;

/// <summary>
/// A base class for a repository.
/// </summary>
/// <typeparam name="TEntity">The type of entity.</typeparam>
/// <typeparam name="TKey">The type of primary key.</typeparam>
public class Repository<TEntity, TKey> : IRepository<TEntity, TKey> where TEntity : IEntity<TKey>
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
        return Db.Connection.QuerySingleOrDefaultAsync<TEntity>(sql, clause);
    }

    public virtual Task<IEnumerable<TEntity>> QueryAsync(object clause)
    {
        var sql = Sql.Select(TableName, clause);
        return Db.Connection.QueryAsync<TEntity>(sql, clause);
    }

    public virtual Task InsertAsync(TEntity entity)
    {
        var sql = Sql.Insert(TableName, entity);
        return Db.Connection.ExecuteAsync(sql, entity);
    }

    public virtual Task InsertAsync(IEnumerable<TEntity> entities)
    {
        if (entities == null || entities.Count() == 0)
            return Task.CompletedTask;
        var sql = Sql.Insert(TableName, typeof(TEntity));
        return Db.Connection.ExecuteAsync(sql, entities);
    }

    public virtual Task<int> UpdateAsync(TEntity entity)
    {
        var updateColumns = Sql.GetParamNames(entity).Where(x => x != "Id");
        var sql = Sql.Update(TableName, updateColumns, new { entity.Id });
        return Db.Connection.ExecuteAsync(sql, entity);
    }

    public virtual Task<int> UpdateAsync(IEnumerable<TEntity> entities)
    {
        var updateColumns = Sql.GetParamNames(typeof(TEntity)).Where(x => x != "Id");
        var sql = Sql.Update(TableName, updateColumns, new { Id = default(long) });
        return Db.Connection.ExecuteAsync(sql, entities);
    }

    public virtual Task<int> UpdateAsync(object update, object clause)
    {
        var sql = Sql.Update(TableName, update, clause);
        return Db.Connection.ExecuteAsync(sql, Sql.MergeParams(update, clause));
    }

    public virtual Task<int> DeleteAsync(TKey id)
    {
        return DeleteAsync(new { Id = id });
    }

    public virtual Task<int> DeleteAsync(object clause)
    {
        var sql = Sql.Delete(TableName, clause);
        return Db.Connection.ExecuteAsync(sql, clause);
    }

    #endregion
}

/// <summary>
/// A base class for a repository.
/// </summary>
/// <typeparam name="TEntity">The type of entity.</typeparam>
public class Repository<TEntity> : Repository<TEntity, long> where TEntity : IEntity<long>
{
    public Repository(Db db) : base(db)
    {
    }
}
