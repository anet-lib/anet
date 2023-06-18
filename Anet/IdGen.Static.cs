﻿namespace Anet;

public partial class IdGen
{
    private static IdGen _instance;

    internal static void SetDefaultOptions(Action<IdGenOptions> configure)
    {
        if (_instance != null)
            throw new InvalidOperationException("Can't set machine id twice.");

        var options = new IdGenOptions();
        configure?.Invoke(options);

        _instance = new IdGen(options);
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
