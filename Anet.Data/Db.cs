using Microsoft.Extensions.Logging;
using System.Data;

namespace Anet.Data;

public class Db : IDbConnection
{
    public static ILogger<Db> Logger { get; internal set; }

    private readonly IDbConnection _connection;

    public Db(IDbConnection connection)
    {
        _connection = connection;
    }

    public string ConnectionString { get => _connection.ConnectionString; set => _connection.ConnectionString = value; }
    public int ConnectionTimeout => _connection.ConnectionTimeout;
    public string Database => _connection.Database;
    public ConnectionState State => _connection.State;
    public IDbTransaction BeginTransaction() => _connection.BeginTransaction();
    public IDbTransaction BeginTransaction(IsolationLevel il) => _connection.BeginTransaction(il);
    public void ChangeDatabase(string databaseName) => _connection.ChangeDatabase(databaseName);
    public void Close() => _connection.Close();
    public IDbCommand CreateCommand() => _connection.CreateCommand();
    public void Open() => _connection.Open();
    public void Dispose() => _connection.Dispose();
}
