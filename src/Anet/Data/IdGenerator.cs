using System;
using System.Diagnostics;
using System.Threading;

namespace Anet.Data
{
    /// <summary>
    /// 基于Twitter Snowflake算法的Id生成器。
    /// 格式：{41位时间戳, 0-10位机器码, 0-12位递增系列号}。
    /// 注意：程序启动前请确保系统时间正确。
    /// </summary>
    public static class IdGenerator
    {
        private const byte TIMESTAMP_BITS = 41;
        
        private static long _machineId = 0;
        private static byte _machineIdBits = 0;
        private static long _sequence = 0;
        private static byte _sequenceBits = 0;
        private static long _maxSequence = 0;
        private static long _lastTimestamp = 0;

        private static readonly long EpochTicks = new DateTime(2017, 1, 1, 0, 0, 0, DateTimeKind.Utc).Ticks;
        private static readonly long OffsetTicks = DateTime.UtcNow.Ticks - EpochTicks;

        private static readonly Stopwatch _stopwatch = Stopwatch.StartNew();
        private static readonly Object _lockObject = new Object();

        static IdGenerator()
        {
            Config(10, 12);
        }

        private static long GetTimestampNow()
        {
            return (OffsetTicks + _stopwatch.Elapsed.Ticks) / 10000L; // 10000 = TimeSpan.FromMilliseconds(1).Ticks
        }

        private static long GetNextTimestamp()
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

        // 获取指定长度二进制的最大整型数。例如：5 返回 000..011111。
        private static long GetMaxOfBits(byte bits)
        {
            return (1L << bits) - 1; // 或 -1 ^ -1 << bits
        }

        /// <summary>
        /// 配置ID生成器
        /// </summary>
        /// <param name="machineIdBits">机器码位数（0-10之间）</param>
        /// <param name="sequenceBits">序列号位数（0-12之间）</param>
        public static void Config(byte machineIdBits = 10, byte sequenceBits = 12)
        {
            if (machineIdBits > 10)
                throw new ArgumentOutOfRangeException(nameof(machineIdBits), "机器码位数必须小于10。");
            if (sequenceBits > 12)
                throw new ArgumentOutOfRangeException(nameof(sequenceBits), "序列号位数必须小于12。");

            _machineIdBits = machineIdBits;
            _sequenceBits = sequenceBits;
            _maxSequence = GetMaxOfBits(_sequenceBits);
        }

        /// <summary>
        /// 设置机器码
        /// </summary>
        /// <param name="machineId">机器码</param>
        public static void SetMachineId(byte machineId)
        {
            var maxMachineId = GetMaxOfBits(machineId);
            if (machineId > maxMachineId)
                throw new ArgumentOutOfRangeException(nameof(machineId), $"机器码超过了最大值{maxMachineId}。");
            _machineId = machineId;
        }

        /// <summary>
        /// 创建新的ID
        /// </summary>
        /// <returns>ID</returns>
        public static long NewId()
        {
            lock (_lockObject)
            {
                _lastTimestamp = GetNextTimestamp();
                int timestampShift = _machineIdBits + _sequenceBits;
                int machineIdShift = _sequenceBits;
                return (_lastTimestamp << timestampShift) | (_machineId << machineIdShift) | _sequence;
            }
        }
    }
}
