using System.Data;
using Anet.Entity;

namespace Anet.Data;

public interface IRepository<TEntity, TKey>
    where TEntity : IEntity<TKey>
    where TKey : IEquatable<TKey>
{
    Db Db { get; }
    string TableName { get; }

    IDbTransaction BeginTransaction();
    IDbTransaction BeginTransaction(IsolationLevel il);
    Task<IEnumerable<TEntity>> QueryAsync(object clause);
    Task<int> DeleteAsync(object clause);
    Task<int> DeleteAsync(TKey id);
    Task<TEntity> FindAsync(object clause);
    Task<TEntity> FindAsync(TKey id);
    Task InsertAsync(TEntity entity);
    Task InsertAsync(IEnumerable<TEntity> entities);
    Task<int> UpdateAsync(object update, object clause);
    Task<int> UpdateAsync(TEntity entity);
    Task<int> UpdateAsync(IEnumerable<TEntity> entities);
}

public interface IRepository<TEntity> : IRepository<TEntity, long>
    where TEntity : IEntity<long>
{
}
