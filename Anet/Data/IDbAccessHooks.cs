using System.Data.Common;

namespace Anet.Data;

public interface IDbAccessHooks
{
    void ConnectionOpened(DbConnection connection, long? elapsed);
    void ConnectionClosed(DbConnection connection, long? elapsed);
    void CommandExecuted(DbCommand command, long? elapsed);
}
