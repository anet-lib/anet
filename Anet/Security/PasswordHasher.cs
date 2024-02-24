using System.Runtime.CompilerServices;
using System.Security.Cryptography;

namespace Anet.Security;

/// <summary>
/// Salted Hasher
/// <para>PBKDF2 with HMAC-SHA1, 128-bit salt, 256-bit subkey, 1000 iterations.</para>
/// <para>Format: { salt, subkey }</para>
/// </summary>
public static class PasswordHasher
{
    private const int _salt_size = 128 / 8;          // 128 bits
    private const int _subkey_size = 256 / 8;        // 256 bits
    private const int _pbkdf2_iteration = 1000;      // default for Rfc2898DeriveBytes.

    public static readonly HashAlgorithmName DefaultAlgorithm = HashAlgorithmName.MD5;

    /// <summary>
    /// Hash the password using <see cref="DefaultAlgorithm"/>.
    /// </summary>
    /// <param name="password">The password to hash.</param>
    /// <returns>Salted hash result.</returns>
    public static string Hash(string password)
    {
        return Hash(password, DefaultAlgorithm);
    }

    /// <summary>
    /// Hash the password.
    /// </summary>
    /// <param name="password">The password to hash.</param>
    /// <param name="algorithm">The hash algorithm to use to derive the key.</param>
    /// <returns>Salted hash result.</returns>
    public static string Hash(string password, HashAlgorithmName algorithm)
    {
        using var rng = RandomNumberGenerator.Create();
        var salt = new byte[_salt_size];
        rng.GetBytes(salt);

        byte[] subkey = Pbkdf2(password, salt, algorithm);

        var hash = new byte[_salt_size + _subkey_size];
        Buffer.BlockCopy(salt, 0, hash, 0, _salt_size);
        Buffer.BlockCopy(subkey, 0, hash, _salt_size, _subkey_size);

        return Convert.ToBase64String(hash);
    }

    /// <summary>
    /// Validates a password given a salt and a hash using <see cref="DefaultAlgorithm"/>.
    /// </summary>
    /// <param name="password">The password to check.</param>
    /// <param name="hash">A hash of the correct password.</param>
    /// <returns>True if the password is correct. False otherwise.</returns>
    public static bool Verify(string password, string hash)
    {
        return Verify(password, hash, DefaultAlgorithm);
    }

    /// <summary>
    /// Validates a password given a salt and a hash.
    /// </summary>
    /// <param name="password">The password to check.</param>
    /// <param name="hash">A hash of the correct password.</param>
    /// <param name="algorithm">The hash algorithm to use to derive the key.</param>
    /// <returns>True if the password is correct. False otherwise.</returns>
    public static bool Verify(string password, string hash, HashAlgorithmName algorithm)
    {
        byte[] decoded = Convert.FromBase64String(hash);
        if (decoded.Length != _salt_size + _subkey_size)
        {
            return false;
        }

        byte[] salt = new byte[_salt_size];
        Buffer.BlockCopy(decoded, 0, salt, 0, _salt_size);

        byte[] expectedSubkey = new byte[_subkey_size];
        Buffer.BlockCopy(decoded, _salt_size, expectedSubkey, 0, _subkey_size);

        byte[] actualSubkey = Pbkdf2(password, salt, algorithm);
        return ByteArraysEqual(expectedSubkey, actualSubkey);
    }

    // Computes the PBKDF2-SHA1 hash of a password.
    private static byte[] Pbkdf2(string password, byte[] salt, HashAlgorithmName algorithm)
    {
        using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, _pbkdf2_iteration, algorithm);
        return pbkdf2.GetBytes(_subkey_size);
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
