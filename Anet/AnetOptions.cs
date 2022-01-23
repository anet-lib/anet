namespace Anet;

public class AnetOptions
{
    /// <summary>
    /// 是否启用默认的ID生成器
    /// </summary>
    /// <param name="machineId">当前机器码（唯一机会编号）</param>
    /// <param name="machineIdBits">机器码位数（0-10之间）</param>
    /// <param name="sequenceBits">
    /// 序列号位数（值在0-20之间）
    /// 注意：
    /// 1. 并发量越大，此值也要越大，例如：10 可以 1 秒内生成 2^10=1024 个 ID。
    /// 2. 每台机器此参数务必相同。
    /// </param>
    public void EnableDefaultIdGen(
        ushort machineId,
        byte machineIdBits = IdGen.DefaultMachineIdBits,
        byte sequenceBits = IdGen.DefaultSequenceBits)
    {
        IdGen.Init(machineId, machineIdBits, sequenceBits);
    }
}
