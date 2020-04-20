using Anet.Entity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Anet.Data
{
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

        public void FillUpdateAudit(TEntity entity, TKey operatorId = default)
        {
            if (entity is IAuditEntity<TKey> audit)
            {
                audit.UpdatedAt = DateTime.Now;
            }
            if (entity is IFullAuditEntity<TKey> fullAudit)
            {
                fullAudit.UpdatedBy = operatorId;
            }
        }

        public void FillCreateAudit(TEntity entity, TKey operatorId = default)
        {
            if (entity is IAuditEntity<TKey> audit)
            {
                audit.CreatedAt = DateTime.Now;
            }
            if (entity is IFullAuditEntity<TKey> fullAudit)
            {
                fullAudit.CreatedBy = operatorId;
            }
            FillUpdateAudit(entity, operatorId);
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

        public virtual Task InsertAsync(TEntity entity, TKey operatorId = default)
        {
            FillCreateAudit(entity, operatorId);
            var sql = Sql.Insert(TableName, entity);
            return Db.ExecuteAsync(sql, entity);
        }

        public virtual Task InsertAsync(IEnumerable<TEntity> entities, TKey operatorId = default)
        {
            if (entities == null || entities.Count() == 0)
                return Task.CompletedTask;
            foreach (var ent in entities)
                FillCreateAudit(ent, operatorId);
            var sql = Sql.Insert(TableName, typeof(TEntity));
            return Db.ExecuteAsync(sql, entities);
        }

        public virtual Task<int> UpdateAsync(TEntity entity, TKey operatorId = default)
        {
            FillUpdateAudit(entity, operatorId);
            var updateColumns = Sql.GetParamNames(entity).Where(x => x != "Id");
            var sql = Sql.Update(TableName, updateColumns, new { entity.Id });
            return Db.ExecuteAsync(sql, entity);
        }

        public virtual Task<int> UpdateAsync(IEnumerable<TEntity> entities, TKey operatorId = default)
        {
            foreach (var ent in entities)
                FillUpdateAudit(ent, operatorId);
            var updateColumns = Sql.GetParamNames(typeof(TEntity)).Where(x => x != "Id");
            var sql = Sql.Update(TableName, updateColumns, new { Id = default(long) });
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
    public class Repository<TEntity> : Repository<TEntity, long> where TEntity : IEntity<long>
    {
        public Repository(Db db) : base(db)
        {
        }
    }
}
