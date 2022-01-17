using System.Data;
using System.Data.Common;
using System.Diagnostics;

namespace Anet.Data;

internal class AnetDbConnection : DbConnection
{
    private DbConnection _connection;
    private readonly IDbAccessHooks _hooks;

    public AnetDbConnection(DbConnection connection, IDbAccessHooks hooks)
    {
        _connection = connection;
        _hooks = hooks;
    }

    /// <summary>
    /// Whether to measure database access elapsed time, in milliseconds.
    /// </summary>
    internal bool MetricsEnabled { get; set; }

    internal void SetConnection(DbConnection connection) => 
        _connection = connection;

    public override void Close()
    {
        Stopwatch sw = MetricsEnabled ? Stopwatch.StartNew() : null;
        try
        {
            _connection.Close();
        }
        finally
        {
            _hooks.ConnectionClosed(this, sw?.ElapsedMilliseconds);
        }
    }

    public override void Open()
    {
        Stopwatch sw = MetricsEnabled ? Stopwatch.StartNew() : null;
        try
        {
            _connection.Open();
        }
        finally
        {
            _hooks.ConnectionOpened(this, sw?.ElapsedMilliseconds);
        }
    }

    public override async Task OpenAsync(CancellationToken cancellationToken)
    {
        Stopwatch sw = MetricsEnabled ? Stopwatch.StartNew() : null;
        try
        {
            await _connection.OpenAsync(cancellationToken);
        }
        finally
        {
            _hooks.ConnectionOpened(this, sw?.ElapsedMilliseconds);
        }
    }

    protected override DbCommand CreateDbCommand() => new AnetDbCommand(this, _connection.CreateCommand(), _hooks);

    public override string ConnectionString { get => _connection.ConnectionString; set => _connection.ConnectionString = value; }
    public override int ConnectionTimeout => _connection.ConnectionTimeout;
    public override string Database => _connection.Database;
    public override string DataSource => _connection.DataSource;
    public override string ServerVersion => _connection.ServerVersion;
    public override ConnectionState State => _connection.State;
    public override void ChangeDatabase(string databaseName) => _connection.ChangeDatabase(databaseName);
    protected override DbTransaction BeginDbTransaction(IsolationLevel isolationLevel) => _connection.BeginTransaction(isolationLevel);
    protected override bool CanRaiseEvents => false;
    public override void EnlistTransaction(System.Transactions.Transaction transaction) => _connection.EnlistTransaction(transaction);
    public override DataTable GetSchema() => _connection.GetSchema();
    public override DataTable GetSchema(string collectionName) => _connection.GetSchema(collectionName);
    public override DataTable GetSchema(string collectionName, string[] restrictionValues) => _connection.GetSchema(collectionName, restrictionValues);

    protected override void Dispose(bool disposing)
    {
        if (disposing)
            _connection?.Dispose();
        base.Dispose(disposing);
    }
}
