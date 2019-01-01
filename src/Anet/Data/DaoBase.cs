using System;
using System.Data;

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
    }

    /// <summary>
    /// Data Access Object
    /// </summary>
    /// <typeparam name="TEntity">The type of entity.</typeparam>
    public abstract class DaoBase<TEntity> : DaoBase<Entity, long>
    {
        public DaoBase(Database db) : base(db)
        {
        }
    }
}
