using Anet.Data.Entity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Anet.Data
{
    /// <summary>
    /// Data Access Object
    /// </summary>
    /// <typeparam name="TEntity">The type of entity.</typeparam>
    /// <typeparam name="TKey">The type of primary key.</typeparam>
    public abstract class DaoBase<TEntity, TKey>
        where TEntity : IEntity<TKey>
        where TKey : IEquatable<TKey>
    {
        /// <summary>
        /// Initialize the DAO.
        /// </summary>
        /// <param name="db">The database to access.</param>
        public DaoBase(Database db)
        {
            Db = db;
        }

        /// <summary>
        /// The database to access.
        /// </summary>
        protected Database Db { get; }

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

        public Task<TEntity> FindAsync(object param)
        {
            var sql = Sql.Select(nameof(TEntity), param);
            return Db.QuerySingleOrDefaultAsync<TEntity>(sql, param);
        }

        public Task<IEnumerable<TEntity>> QueryAsync(object param)
        {
            var sql = Sql.Select(nameof(TEntity), param);
            return Db.QueryAsync<TEntity>(sql, param);
        }

        public Task InsertAsync(TEntity entity)
        {
            var sql = Sql.Insert(nameof(TEntity), entity);
            return Db.ExecuteAsync(sql, entity);
        }

        public Task<int> UpdateAsync(object updateParam, object clauseParam)
        {
            var sql = Sql.Update(nameof(TEntity), updateParam, clauseParam);
            return Db.ExecuteAsync(sql, Sql.MergeParams(updateParam, clauseParam));
        }

        #endregion
    }

    /// <summary>
    /// Data Access Object
    /// </summary>
    /// <typeparam name="TEntity">The type of entity.</typeparam>
    public abstract class DaoBase<TEntity> : DaoBase<IEntity, long>
    {
        public DaoBase(Database db) : base(db)
        {
        }
    }
}
