namespace Anet;

public class IdGenOptions
{
    public const byte DefaultMachineIdBits = 6;
    public const byte DefaultSequenceBits = 12;

    /// <summary>
    /// 当前机器码（唯一机会编号）
    /// </summary>
    public uint MachineId { get; set; }

    /// <summary>
    /// 机器码位数（0-10之间）
    /// </summary>
    public byte MachineIdBits { get; set; } = DefaultMachineIdBits;

    /// <summary>
    /// 序列号位数（值在0-20之间）
    /// 注意：
    /// 1. 并发量越大，此值也要越大，例如：10 可以 1 秒内生成 2^10=1024 个 ID。
    /// 2. 每台机器此参数务必相同。
    /// </summary>
    public byte SequenceBits { get; set; } = DefaultSequenceBits;
}
