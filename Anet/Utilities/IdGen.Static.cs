namespace Anet;

public partial class IdGen
{
    private static IdGen _instance;

    /// <summary>
    /// Use <see cref="IdGen"/> for generating sequence IDs.
    /// </summary>
    /// <param name="machineId">当前机器码（唯一机会编号）</param>
    /// <param name="machineIdBits">机器码位数（0-10之间）</param>
    /// <param name="sequenceBits">
    /// 序列号位数（0-20之间）
    /// 注意：
    /// 1. 并发量越大，此值也要越大，例如：10 可以 1 秒内生成 2^10=1024 个 ID。
    /// 2. 每台机器此参数务必相同。
    /// </param>
    public static void Init(
        ushort machineId,
        byte machineIdBits = DefaultMachineIdBits,
        byte sequenceBits = DefaultSequenceBits)
    {
        if (_instance != null)
            throw new InvalidOperationException("Can't set machine id twice.");
        _instance = new IdGen(machineId, machineIdBits, sequenceBits);
    }

    /// <summary>
    /// Generate a new sequence id.
    /// </summary>
    /// <returns>The generated id.</returns>
    public static long NewId()
    {
        if (_instance == null)
            throw new Exception("The IdGen has no default instance.");
        return _instance.NewSequenceId();
    }

    // 获取指定长度二进制的最大整型数。例如：5 返回 000..011111。
    private static long GetMaxOfBits(byte bits)
    {
        return (1L << bits) - 1; // 或 -1 ^ -1 << bits
    }

    // 获取数字的二进制位长度。例如 5 的位长度是 3。
    private static int GetBitsLength(long number)
    {
        return (int)Math.Log(number, 2) + 1;
    }
}
