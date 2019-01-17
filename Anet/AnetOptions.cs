namespace Anet
{
    public class AnetOptions
    {
        /// <summary>
        /// 配置ID生成器
        /// </summary>
        /// <param name="machineIdBits">机器码位数（0-10之间）</param>
        /// <param name="sequenceBits">序列号位数（0-12之间）</param>
        public void ConfigIdGenerator(byte machineIdBits = 10, byte sequenceBits = 12)
        {
            IdGenerator.Config(machineIdBits, sequenceBits);
        }
    }
}
