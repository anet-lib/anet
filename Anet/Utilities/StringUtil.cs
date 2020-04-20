using System;
using System.Collections.Generic;
using System.Linq;

namespace Anet
{
    public static class StringUtil
    {
        public const string BASE36 = "0123456789abcdefghijklmnopqrstuvwxyz";
        public const string BASE64 = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz-_";
        public const string BASE64_CLUTTER = "r4DMplQZJXPqe_bE0WSjBn7wVzmN2Gc6THCAKuts9LFk-gy5RovixI1aOf8UhdY3";
        public const string BASE_DEFAULT = BASE36;

        public static string BaseEncode(long number, string baseChars = BASE_DEFAULT)
        {
            var stack = new Stack<char>();
            while (number >= baseChars.Length)
            {
                var remainder = (byte)(number % baseChars.Length);
                stack.Push(baseChars[remainder]);
                number /= baseChars.Length;
            }
            stack.Push(baseChars[(byte)number]);
            return new string(stack.ToArray());
        }

        public static long BaseDecode(string text, string baseChars = BASE_DEFAULT)
        {
            long result = 0;
            for (int i = 0; i < text.Length; i++)
            {
                result += (long)Math.Pow(baseChars.Length, text.Length - i - 1) * baseChars.IndexOf(text[i]);
            }
            return result;
        }

        /// <summary>
        /// 混淆数字为字符串
        /// </summary>
        public static string Mixup(string text)
        {
            int sum = 0;
            foreach (char c in text)
            {
                sum += c;
            }
            int x = sum % text.Length;
            char[] arr = text.ToCharArray();
            char[] newArr = new char[arr.Length];
            Array.Copy(arr, x, newArr, 0, text.Length - x);
            Array.Copy(arr, 0, newArr, text.Length - x, x);
            return new string(newArr);
        }

        /// <summary>
        /// 解开混淆字符串
        /// </summary>
        public static string UnMixup(string cipher)
        {
            int sum = 0;
            foreach (char c in cipher)
            {
                sum += c;
            }
            int x = cipher.Length - sum % cipher.Length;
            char[] arr = cipher.ToCharArray();
            char[] newArr = new char[arr.Length];
            Array.Copy(arr, x, newArr, 0, cipher.Length - x);
            Array.Copy(arr, 0, newArr, cipher.Length - x, x);
            return new string(newArr);
        }

        /// <summary>
        /// 拼接多个路径
        /// </summary>
        public static string CombinePath(char separator, params string[] paths)
        {
            Guard.NotNull(separator, nameof(separator));
            Guard.NotNull(paths, nameof(paths));
            for (int i = 0; i < paths.Length; i++)
            {
                if (i != 0)
                    paths[i] = paths[i].TrimStart(separator);
                if (i != paths.Length - 1)
                    paths[i] = paths[i].TrimEnd(separator);
            }
            return string.Join(separator, paths);
        }

        /// <summary>
        /// 拼接多个路径
        /// 以路径参数中首次出现的分隔符（“/"或“\”）作为拼接分隔符
        /// 如果路径参数中无分隔符，则使用当前系统路径分隔符
        /// </summary>
        public static string CombinePath(params string[] paths)
        {
            Guard.NotNull(paths, nameof(paths));
            char seprator = string.Join("", paths).ToCharArray().FirstOrDefault(c => c == '/' || c == '\\');
            if (seprator == default(char))
            {
                seprator = System.IO.Path.DirectorySeparatorChar;
            }
            return CombinePath(seprator, paths);
        }

        /// <summary>
        /// 拼接URL路径
        /// </summary>
        public static string CombineUrl(params string[] paths)
        {
            return CombinePath('/', paths);
        }

        public static string GenRandomString(int size, bool useUpper = true, bool useLower = true, bool useNumber = true)
        {
            string alphabet = "";
            if (useUpper) alphabet += "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            if (useLower) alphabet += "abcdefghijklmnopqrstuvwyxz";
            if (useNumber) alphabet += "0123456789";

            var rand = new Random();
            char[] chars = new char[size];
            for (int i = 0; i < size; i++)
            {
                chars[i] = alphabet[rand.Next(alphabet.Length)];
            }
            return new string(chars);
        }
    }
}
