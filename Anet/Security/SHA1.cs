using System.Text;

using Hasher = System.Security.Cryptography.SHA1;

namespace Anet.Security;

public static class SHA1
{
    public static string Hash(string input)
    {
        var result = Hasher.HashData(Encoding.UTF8.GetBytes(input));
        return HashUtil.BytesToString(result);
    }

    public static string Hash(byte[] buffer)
    {
        var result = Hasher.HashData(buffer);
        return HashUtil.BytesToString(result);
    }

    public static string Hash(Stream stream)
    {
        using var sha1 = Hasher.Create();
        byte[] result = sha1.ComputeHash(stream);
        return HashUtil.BytesToString(result);
    }
}
