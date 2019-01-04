using System;

namespace Anet
{
    public static class StringUtil
    {
        public static string GenerateRandomString(int size)
        {
            string alphabet = "abcdefghijklmnopqrstuvwyxzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
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
