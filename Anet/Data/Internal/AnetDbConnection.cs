using System.Data;
using System.Data.Common;
using System.Diagnostics;

namespace Anet.Data;

internal class AnetDbConnection : DbConnection
{
    private readonly IDbAccessHooks _hooks;

    public AnetDbConnection(DbConnection connection, IDbAccessHooks hooks)
    {
        Connection = connection;
        _hooks = hooks;
    }

    internal DbConnection Connection { get; set; }

    /// <summary>
    /// Whether to measure database access elapsed time, in milliseconds.
    /// </summary>
    internal bool MetricsEnabled { get; set; }

    public override void Close()
    {
        Stopwatch sw = MetricsEnabled ? Stopwatch.StartNew() : null;
        try
        {
            Connection.Close();
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
            Connection.Open();
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
            await Connection.OpenAsync(cancellationToken);
        }
        finally
        {
            _hooks.ConnectionOpened(this, sw?.ElapsedMilliseconds);
        }
    }

    protected override DbCommand CreateDbCommand() => new AnetDbCommand(this, Connection.CreateCommand(), _hooks);

    public override string ConnectionString { get => Connection.ConnectionString; set => Connection.ConnectionString = value; }
    public override int ConnectionTimeout => Connection.ConnectionTimeout;
    public override string Database => Connection.Database;
    public override string DataSource => Connection.DataSource;
    public override string ServerVersion => Connection.ServerVersion;
    public override ConnectionState State => Connection.State;
    public override void ChangeDatabase(string databaseName) => Connection.ChangeDatabase(databaseName);
    protected override DbTransaction BeginDbTransaction(IsolationLevel isolationLevel) => Connection.BeginTransaction(isolationLevel);
    protected override bool CanRaiseEvents => false;
    public override void EnlistTransaction(System.Transactions.Transaction transaction) => Connection.EnlistTransaction(transaction);
    public override DataTable GetSchema() => Connection.GetSchema();
    public override DataTable GetSchema(string collectionName) => Connection.GetSchema(collectionName);
    public override DataTable GetSchema(string collectionName, string[] restrictionValues) => Connection.GetSchema(collectionName, restrictionValues);

    protected override void Dispose(bool disposing)
    {
        if (disposing)
            Connection?.Dispose();
        base.Dispose(disposing);
    }
}
