using System;
using System.IO;
using System.Text;

namespace Anet.Security
{
    public abstract class MD5
    {
        public static string Compute(string input)
        {
            using var md5 = System.Security.Cryptography.MD5.Create();
            var result = md5.ComputeHash(Encoding.UTF8.GetBytes(input));
            return HashToCompactString(result);
        }

        public static string Compute(byte[] buffer)
        {
            using var md5 = System.Security.Cryptography.MD5.Create();
            var result = md5.ComputeHash(buffer);
            return HashToCompactString(result);
        }

        public static string Compute(Stream stream)
        {
            using var md5 = System.Security.Cryptography.MD5.Create();
            var result = md5.ComputeHash(stream);
            return HashToCompactString(result);
        }

        private static string HashToCompactString(byte[] hash)
        {
            return BitConverter.ToString(hash).Replace("-", "").ToLower();
        }
    }
}
