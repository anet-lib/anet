using System.Data;
using System.Data.Common;
using System.Diagnostics;

namespace Anet.Data;

internal class AnetDbCommand : DbCommand
{
    private readonly AnetDbConnection _connection;
    private readonly DbCommand _command;
    private readonly IDbAccessHooks _hooks;

    public AnetDbCommand(
        AnetDbConnection connection,
        DbCommand command,
        IDbAccessHooks hooks)
    {
        _connection = connection;
        _command = command;
        _hooks = hooks;
    }

    protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior)
    {
        var sw = _connection.MetricsEnabled ? Stopwatch.StartNew() : null;
        try
        {
            return _command.ExecuteReader(behavior);
        }
        finally
        {
            _hooks.CommandExecuted(this, sw?.ElapsedMilliseconds);
        }
    }

    protected override async Task<DbDataReader> ExecuteDbDataReaderAsync(
        CommandBehavior behavior, CancellationToken cancellationToken)
    {
        var sw = _connection.MetricsEnabled ? Stopwatch.StartNew() : null;
        try
        {
            return await _command.ExecuteReaderAsync(behavior, cancellationToken);
        }
        finally
        {
            _hooks.CommandExecuted(this, sw?.ElapsedMilliseconds);
        }
    }

    public override int ExecuteNonQuery()
    {
        var sw = _connection.MetricsEnabled ? Stopwatch.StartNew() : null;
        try
        {
            return _command.ExecuteNonQuery();
        }
        finally
        {
            _hooks.CommandExecuted(this, sw?.ElapsedMilliseconds);
        }
    }

    public override async Task<int> ExecuteNonQueryAsync(CancellationToken cancellationToken)
    {
        var sw = _connection.MetricsEnabled ? Stopwatch.StartNew() : null;
        try
        {
            return await _command.ExecuteNonQueryAsync(cancellationToken);
        }
        finally
        {
            _hooks.CommandExecuted(this, sw?.ElapsedMilliseconds);
        }
    }

    public override object ExecuteScalar()
    {
        var sw = _connection.MetricsEnabled ? Stopwatch.StartNew() : null;
        try
        {
            return _command.ExecuteScalar();
        }
        finally
        {
            _hooks.CommandExecuted(this, sw?.ElapsedMilliseconds);
        }
    }

    public override async Task<object> ExecuteScalarAsync(CancellationToken cancellationToken)
    {
        var sw = _connection.MetricsEnabled ? Stopwatch.StartNew() : null;
        try
        {
            return await _command.ExecuteScalarAsync(cancellationToken);
        }
        finally
        {
            _hooks.CommandExecuted(this, sw?.ElapsedMilliseconds);
        }
    }

    public override string CommandText { get => _command.CommandText; set => _command.CommandText = value; }
    public override int CommandTimeout { get => _command.CommandTimeout; set => _command.CommandTimeout = value; }
    public override CommandType CommandType { get => _command.CommandType; set => _command.CommandType = value; }
    protected override DbConnection DbConnection { get => _connection; set => _connection.Connection = value; }
    protected override DbTransaction DbTransaction { get => _command.Transaction; set => _command.Transaction = value; }
    public override bool DesignTimeVisible { get => _command.DesignTimeVisible; set => _command.DesignTimeVisible = value; }
    public override UpdateRowSource UpdatedRowSource { get => _command.UpdatedRowSource; set => _command.UpdatedRowSource = value; }
    protected override DbParameterCollection DbParameterCollection => _command.Parameters;
    public override void Cancel() => _command.Cancel();
    public override void Prepare() => _command.Prepare();
    protected override DbParameter CreateDbParameter() => _command.CreateParameter();

    protected override void Dispose(bool disposing)
    {
        if (disposing)
            _command?.Dispose();
        base.Dispose(disposing);
    }
}

