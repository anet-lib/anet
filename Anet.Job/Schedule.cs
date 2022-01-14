namespace Anet.Job;

internal class Schedule
{
    public Type JobType { get; internal set; }

    public TimeSpan Interval { get; internal set; }

    public DateTime NextRunTime { get; internal set; }
}

