using Microsoft.Extensions.Logging;

namespace Anet.Data.Logging;

public class LoggingOptions
{
    public LogLevel LogLevel { get; set; } = LogLevel.Information;
    public string OpenConnectionMessage { get; set; } = "Connection: open, Elapsed: {elapsed} ms";
    public string CloseConnectionMessage { get; set; } = "Connection: close, Elapsed: {elapsed} ms";
    public string ExecuteCommandMessage { get; set; } = "\nCommand:\n{command}\nParameters:\n{params}\nElapsed: {elapsed} ms";

    public bool LogSensitiveData { get; set; }
}

