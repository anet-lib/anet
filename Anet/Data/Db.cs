using System.Data;
using Microsoft.Extensions.Logging;

namespace Anet.Data;

public class Db : IDisposable
{
    private readonly ILogger<Db> _logger;

    public Db(DbDialect dialect, 
        IDbConnection connection, 
        DbOptions options = null,
        ILogger<Db> logger = null)
    {
        Dialect = dialect;
        Connection = connection;
        Options = options ?? new DbOptions();
        _logger = logger;
    }

    public DbDialect Dialect { get; }

    public DbOptions Options { get; }

    public SqlString NewSql(string value = null) => new(Dialect, value);

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

    private IDbTransaction _transaction;
    
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
        _logger?.Log(Options.LogLevel, "Connection: disposed");
    }
}

