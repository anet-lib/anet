using System.Data;
using System.Data.Common;
using Microsoft.Extensions.Logging;

namespace Anet.Data;

public class Db : IDisposable
{
    private IDbTransaction _transaction;

    public Db(IDbConnection connection, DbOptions options, ILogger logger)
    {
        Options = options ?? new DbOptions();
        if (Options.Dialect == DbDialect.Auto)
        {
            Options.Dialect = ResolveDialect(connection);
        }

        if (logger == null)
        {
            Connection = connection;
        }
        else
        {
            var hooks = new LoggingHooks(logger, options);
            Connection = new AnetDbConnection(connection as DbConnection, hooks)
            {
                MetricsEnabled = options.EnableMetrics
            };
        }

        Logger = logger;
    }

    internal static DbDialect ResolveDialect(IDbConnection connection)
    {
        var name = connection.GetType().Name;
        if (name.StartsWith("MySql", StringComparison.OrdinalIgnoreCase))
            return DbDialect.MySQL;
        else if (name.StartsWith("Npgsql", StringComparison.OrdinalIgnoreCase))
            return DbDialect.PostgreSQL;
        else if (name.StartsWith("Oracle", StringComparison.OrdinalIgnoreCase))
            return DbDialect.Oracle;
        else if (name.StartsWith("Sqlite", StringComparison.OrdinalIgnoreCase))
            return DbDialect.SQLite;
        else if (name.StartsWith("Sql"))
            return DbDialect.SQLServer;
        throw new NotSupportedException($"Cannot recognize DbDialect of '{name}', please specify the DbDialect parameter manually.");
    }

    public ILogger Logger { get; }

    public DbOptions Options { get; }

    public SqlString NewSql(string value = null) => new(Options.Dialect, value);

    /// <summary>
    /// The current transaction to use, if any.
    /// </summary>
    public IDbConnection Connection { get; }

    /// <summary>
    /// Manually open the connection, later command will use the same connection.
    /// Otherwise, Dapper auto open and close the connection every command by default.
    /// </summary>
    public void OpenConnection() => Connection.Open();

    /// <summary>
    /// Number of seconds before command execution timeout.
    /// </summary>
    public int? CommandTimeout { get; set; }

    public IDbTransaction Transaction
    {
        // return null if transaction is disposed.
        get => _transaction == null || _transaction.Connection == null ? null : _transaction;
        private set => _transaction = value;
    }

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
        // Auto open connection.
        if (Connection.State == ConnectionState.Closed)
            Connection.Open();
        Transaction = Connection.BeginTransaction(il);
        return Transaction;
    }

    public void Dispose()
    {
        Transaction?.Dispose();
        Connection?.Dispose();
        GC.SuppressFinalize(this);
        Logger?.Log(Options.LogLevel, "Connection: disposed");
    }
}

