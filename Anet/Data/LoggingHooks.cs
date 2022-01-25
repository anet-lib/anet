using System.Data.Common;
using Microsoft.Extensions.Logging;

using static System.Environment;

namespace Anet.Data;

internal class LoggingHooks : IDbAccessHooks
{
    static readonly string _openConnectionMessage = "Connection: open, Elapsed: {elapsed} ms";
    static readonly string _closeConnectionMessage = "Connection: close, Elapsed: {elapsed} ms";
    static readonly string _executeCommandMessage = $"{{command}}{NewLine}Params: {{params}}{NewLine}Elapsed: {{elapsed}} ms";

    private readonly ILogger _logger;
    private readonly DbOptions _options;

    public LoggingHooks(ILogger logger, DbOptions options)
    {
        _logger = logger;
        _options = options;
    }

    public void ConnectionOpened(DbConnection connection, long? elapsed) =>
        _logger.Log(
            _options.LogLevel,
            _openConnectionMessage,
            elapsed);

    public void ConnectionClosed(DbConnection connection, long? elapsed) =>
        _logger.Log(
            _options.LogLevel,
            _closeConnectionMessage,
            elapsed);

    public void CommandExecuted(DbCommand command, long? elapsed)
    {
        _logger.Log(
            _options.LogLevel,
            _executeCommandMessage,
            command.CommandText,
            command.GetParameters(hideValues: !_options.LogSensitiveData),
            elapsed);
    }
        
}

