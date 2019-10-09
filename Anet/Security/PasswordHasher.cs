using System;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;

namespace Anet.Security
{
    /// <summary>
    /// Salted Hasher
    /// <para>PBKDF2 with HMAC-SHA1, 128-bit salt, 256-bit subkey, 1000 iterations.</para>
    /// <para>Format: { salt, subkey }</para>
    /// </summary>
    public class PasswordHasher
    {
        private const int SALT_SIZE = 128 / 8;          // 128 bits
        private const int SUBKEY_SIZE = 256 / 8;        // 256 bits
        private const int PBKDF2_ITERATION = 1000;      // default for Rfc2898DeriveBytes.

        /// <summary>
        /// Hash the password.
        /// </summary>
        /// <param name="password">The password to hash.</param>
        /// <returns>Salted hash result.</returns>
        public static string HashPassword(string password)
        {
            using var rng = RandomNumberGenerator.Create();
            var salt = new byte[SALT_SIZE];
            rng.GetBytes(salt);

            byte[] subkey = Pbkdf2(password, salt);

            var hash = new byte[SALT_SIZE + SUBKEY_SIZE];
            Buffer.BlockCopy(salt, 0, hash, 0, SALT_SIZE);
            Buffer.BlockCopy(subkey, 0, hash, SALT_SIZE, SUBKEY_SIZE);

            return Convert.ToBase64String(hash);
        }

        /// <summary>
        /// Validates a password given a salt and a hash.
        /// </summary>
        /// <param name="password">The password to check.</param>
        /// <param name="hash">A hash of the correct password.</param>
        /// <returns>True if the password is correct. False otherwise.</returns>
        public static bool VerifyPassword(string password, string hash)
        {
            byte[] decoded = Convert.FromBase64String(hash);
            if (decoded.Length != SALT_SIZE + SUBKEY_SIZE)
            {
                return false;
            }

            byte[] salt = new byte[SALT_SIZE];
            Buffer.BlockCopy(decoded, 0, salt, 0, SALT_SIZE);

            byte[] expectedSubkey = new byte[SUBKEY_SIZE];
            Buffer.BlockCopy(decoded, SALT_SIZE, expectedSubkey, 0, SUBKEY_SIZE);

            byte[] actualSubkey = Pbkdf2(password, salt);
            return ByteArraysEqual(expectedSubkey, actualSubkey);
        }

        // Computes the PBKDF2-SHA1 hash of a password.
        private static byte[] Pbkdf2(string password, byte[] salt)
        {
            using var pbkdf2 = new Rfc2898DeriveBytes(password, salt)
            {
                IterationCount = PBKDF2_ITERATION
            };
            return pbkdf2.GetBytes(SUBKEY_SIZE);
        }

        // Compares two byte arrays for equality. The method is specifically written so that the loop is not optimized.
        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private static bool ByteArraysEqual(byte[] a, byte[] b)
        {
            if (a == null && b == null)
            {
                return true;
            }
            if (a == null || b == null || a.Length != b.Length)
            {
                return false;
            }
            var areSame = true;
            for (var i = 0; i < a.Length; i++)
            {
                areSame &= (a[i] == b[i]);
            }
            return areSame;
        }
    }
}
