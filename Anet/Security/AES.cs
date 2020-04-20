using System;
using System.Security.Cryptography;
using System.Text;

namespace Anet.Security
{
    public static class AES
    {
        public static byte[] Encrypt(byte[] data, byte[] key, byte[] iv, CipherMode cipherMode, PaddingMode paddingMode)
        {
            Guard.NotNull(data, nameof(data));
            Guard.NotNull(key, nameof(key));

            using var aes = Aes.Create();
            aes.Key = key;
            aes.Mode = cipherMode;
            aes.Padding = paddingMode;
            aes.IV = iv;

            using var encryptor = aes.CreateEncryptor();
            return encryptor.TransformFinalBlock(data, 0, data.Length);
        }

        public static byte[] Decrypt(byte[] data, byte[] key, byte[] iv, CipherMode cipherMode, PaddingMode paddingMode)
        {
            Guard.NotNull(data, nameof(data));
            Guard.NotNull(key, nameof(key));

            using var aes = Aes.Create();
            aes.Key = key;
            aes.Mode = cipherMode;
            aes.Padding = paddingMode;
            aes.IV = iv;

            using var decryptor = aes.CreateDecryptor();
            return decryptor.TransformFinalBlock(data, 0, data.Length);
        }

        public static string Encrypt(string data, string key, string iv, CipherMode cipherMode = CipherMode.CBC, PaddingMode paddingMode = PaddingMode.PKCS7)
        {
            Guard.NotNull(iv, nameof(iv));

            var dataBytes = Encoding.UTF8.GetBytes(data);
            var keyBytes = Encoding.UTF8.GetBytes(key);
            var ivBytes = Encoding.UTF8.GetBytes(iv);

            var resultBytes = Encrypt(dataBytes, keyBytes, ivBytes, cipherMode, paddingMode);

            return Convert.ToBase64String(resultBytes);
        }

        public static string Decrypt(string data, string key, string iv, CipherMode cipherMode = CipherMode.CBC, PaddingMode paddingMode = PaddingMode.PKCS7)
        {
            Guard.NotNull(iv, nameof(iv));

            var dataBytes = Convert.FromBase64String(data);
            var keyBytes = Encoding.UTF8.GetBytes(key);
            var ivBytes = Encoding.UTF8.GetBytes(iv);

            var resultBytes = Decrypt(dataBytes, keyBytes, ivBytes, cipherMode, paddingMode);

            return Encoding.UTF8.GetString(resultBytes);
        }
    }
}
