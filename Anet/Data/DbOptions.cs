using Microsoft.Extensions.Logging;

namespace Anet.Data;

public class DbOptions
{
    public DbDialect Dialect { get; set; } = DbDialect.Auto;
    public LogLevel LogLevel { get; set; } = LogLevel.Information;
    public bool EnableMetrics { get; set; }
    public bool LogSensitiveData { get; set; }
}
