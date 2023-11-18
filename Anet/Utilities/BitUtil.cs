namespace Anet.Utilities;

public static class BitUtil
{
    /// <summary>
    /// 获取指定长度二进制的最大整型数。例如：5 返回 000..011111。
    /// </summary>
    /// <param name="bits"></param>
    /// <returns></returns>
    public static long GetMaxOfBits(byte bits)
    {
        return (1L << bits) - 1; // 或 -1 ^ -1 << bits
    }

    /// <summary>
    /// 获取数字的二进制位长度。例如 5 的位长度是 3。
    /// </summary>
    /// <param name="number"></param>
    /// <returns></returns>
    public static int GetBitsLength(long number)
    {
        return (int)Math.Log(number, 2) + 1;
    }
}
