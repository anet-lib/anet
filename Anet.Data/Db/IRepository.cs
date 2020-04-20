using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Anet.Entity;

namespace Anet.Data
{
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
        Task InsertAsync(TEntity entity, TKey userId = default);
        Task InsertAsync(IEnumerable<TEntity> entities, TKey userId = default);
        Task<int> UpdateAsync(object update, object clause);
        Task<int> UpdateAsync(TEntity entity, TKey userId = default);
        Task<int> UpdateAsync(IEnumerable<TEntity> entities, TKey userId = default);
    }

    public interface IRepository<TEntity> : IRepository<TEntity, long>
        where TEntity : IEntity<long>
    {
    }
}