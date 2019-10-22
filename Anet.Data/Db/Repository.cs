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
    public abstract class Repository<TEntity, TKey>
        where TEntity : IEntity<TKey>
        where TKey : IEquatable<TKey>
    {
        protected string TableName { get => typeof(TEntity).Name; }

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
        protected Db Db { get; }

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

        public Task<TEntity> FindAsync(TKey id)
        {
            return FindAsync(new { Id = id });
        }

        public Task<TEntity> FindAsync(object clause)
        {
            var sql = Sql.Select(TableName, clause);
            return Db.QuerySingleOrDefaultAsync<TEntity>(sql, clause);
        }

        public Task<IEnumerable<TEntity>> QueryAsync(object clause)
        {
            var sql = Sql.Select(TableName, clause);
            return Db.QueryAsync<TEntity>(sql, clause);
        }

        public Task InsertAsync(TEntity entity)
        {
            var sql = Sql.Insert(TableName, entity);
            return Db.ExecuteAsync(sql, entity);
        }

        public Task<int> UpdateAsync(TEntity entity)
        {
            var updateColumns = Sql.GetParamNames(entity).Where(x => x != "Id");
            var sql = Sql.Update(TableName, updateColumns, new { entity.Id });
            return Db.ExecuteAsync(sql, entity);
        }

        public Task<int> UpdateAsync(object update, object clause)
        {
            var sql = Sql.Update(TableName, update, clause);
            return Db.ExecuteAsync(sql, Sql.MergeParams(update, clause));
        }

        public Task<int> DeleteAsync(TKey id)
        {
            return DeleteAsync(new { Id = id });
        }

        public Task<int> DeleteAsync(object clause)
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
    public abstract class Repository<TEntity> : Repository<TEntity, long>
        where TEntity : IEntity
    {
        public Repository(Db db) : base(db)
        {
        }
    }
}
