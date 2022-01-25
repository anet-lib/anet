using Microsoft.Extensions.Logging;

namespace Anet.Data;

public class DbOptions
{
    public LogLevel LogLevel { get; set; } = LogLevel.Information;
    public bool EnableMetrics { get; set; }
    public bool LogSensitiveData { get; set; }
}
