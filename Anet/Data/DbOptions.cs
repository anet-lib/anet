using Anet.Data.Logging;

namespace Anet.Data;

public class DbOptions
{
    public LoggingOptions LoggingOptions { get; set; }
    public bool EnableMetrics { get; set; }
}
