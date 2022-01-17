using System.Data.Common;
using Microsoft.Extensions.Logging;

namespace Anet.Data.Logging;

internal class LoggingHooks : IDbAccessHooks
{
    private readonly ILogger _logger;
    private readonly LoggingOptions _options;

    public LoggingHooks(ILogger logger, LoggingOptions options)
    {
        _logger = logger;
        _options = options;
    }

    public void ConnectionOpened(DbConnection connection, long? elapsed) =>
        _logger.Log(
            _options.LogLevel,
            _options.OpenConnectionMessage,
            elapsed);

    public void ConnectionClosed(DbConnection connection, long? elapsed) =>
        _logger.Log(
            _options.LogLevel,
            _options.CloseConnectionMessage,
            elapsed);

    public void CommandExecuted(DbCommand command, long? elapsed) =>
        _logger.Log(
            _options.LogLevel,
            _options.ExecuteCommandMessage,
            command.CommandText,
            command.GetParameters(hideValues: !_options.LogSensitiveData),
            elapsed);
}

