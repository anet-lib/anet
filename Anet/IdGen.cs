using System.Diagnostics;
using Anet.Utilities;

namespace Anet;

/// <summary>
/// 类似 Twitter Snowflake(41 + 10 + 12) 算法的 Id 生成器。
/// 格式：{32 位时间戳, 0-10 位机器码, 0-20 位递增系列号}。
/// 注意：程序启动前请确保系统时间正确。
/// </summary>
public partial class IdGen
{
    private readonly long _maxSequence = 0;
    private readonly object _lockObject = new();
    private readonly Stopwatch _stopwatch = Stopwatch.StartNew();

    private long _sequence = 0;
    private long _lastTimestamp = 0;

    // 637134336000000000 = new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc).Ticks
    private static readonly long _offsetTicks = DateTime.UtcNow.Ticks - 637134336000000000;

    private readonly IdGenOptions _options;

    /// <summary>
    /// The constructor of <see cref="IdGen"/>.
    /// </summary>
    public IdGen(IdGenOptions options)
    {
        if (options.SequenceBits > 20)
            throw new ArgumentOutOfRangeException(nameof(options.SequenceBits), "序列号不能超过 20 位。");

        if (options.MachineIdBits > 10)
            throw new ArgumentOutOfRangeException(nameof(options.MachineIdBits), "机器码不能超过 10 位。");

        var maxMachineId = BitUtil.GetMaxOfBits(options.MachineIdBits);
        if (options.MachineId > maxMachineId)
            throw new ArgumentOutOfRangeException(nameof(options.MachineId), $"机器码不能大于 {maxMachineId}。");

        _options = options;
        _maxSequence = BitUtil.GetMaxOfBits(options.SequenceBits);
    }

    private long GetTimestampNow()
    {
        // 10000000 = TimeSpan.FromSeconds(1).Ticks
        return (_offsetTicks + _stopwatch.Elapsed.Ticks) / 10000000L;
    }

    private long GetNextTimestamp()
    {
        long timestamp = GetTimestampNow();
        if (timestamp < _lastTimestamp)
            throw new Exception("新的时间戳比旧的小，请检查系统时间。");

        while (timestamp == _lastTimestamp)
        {
            if (_sequence < _maxSequence)
            {
                _sequence++;
                return timestamp;
            }
            Thread.Sleep(0); // 降低CPU消耗
            timestamp = GetTimestampNow();
        }
        _sequence = 0;

        return timestamp;
    }

    /// <summary>
    /// 生成新的ID
    /// </summary>
    /// <returns>ID</returns>
    public long NewSequenceId()
    {
        lock (_lockObject)
        {
            _lastTimestamp = GetNextTimestamp();

            //int bitsLength = GetBitsLength(_lastTimestamp);
            //Console.WriteLine($"Timestamp bits: {bitsLength}");

            int timestampShift = _options.MachineIdBits + _options.SequenceBits;
            int machineIdShift = _options.SequenceBits;
            return (_lastTimestamp << timestampShift) | (_options.MachineId << machineIdShift) | _sequence;
        }
    }
}
