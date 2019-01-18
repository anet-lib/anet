using System;

namespace Anet
{
    public partial class IdGen
    {
        private static readonly long OffsetTicks = 
            DateTime.UtcNow.Ticks - new DateTime(2018, 1, 1, 0, 0, 0, DateTimeKind.Utc).Ticks;

        private static IdGen Instance;

        /// <summary>
        /// Set the default instance of <see cref="IdGen"/>.
        /// </summary>
        /// <param name="idGen">The instace of <see cref="IdGen"/>.</param>
        public static void SetDefault(IdGen idGen = null)
        {
            if (Instance != null)
                throw new InvalidOperationException("Can't set default instance of IdGen twice.");
            Instance = idGen ?? new IdGen();
        }

        /// <summary>
        /// Generate a new sequence id.
        /// </summary>
        /// <returns>The generated id.</returns>
        public static long NewId()
        {
            if (Instance == null)
                throw new Exception("The IdGen has no default instance.");
            return Instance.NewSequenceId();
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
}
