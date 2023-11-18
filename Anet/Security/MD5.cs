using System.Text;

using SysMD5 = System.Security.Cryptography.MD5;

namespace Anet.Security;

public enum MD5Format
{
    LowerNoHyphen,
    LowerWithHyphen,
    UpperNoHyphen,
    UpperWithHyphen,
}

public static class MD5
{
    public static string Hash(string input, MD5Format format = MD5Format.LowerNoHyphen)
    {
        var result = SysMD5.HashData(Encoding.UTF8.GetBytes(input));
        return HashToString(result, format);
    }

    public static string Hash(byte[] buffer, MD5Format format = MD5Format.LowerNoHyphen)
    {
        var result = SysMD5.HashData(buffer);
        return HashToString(result, format);
    }

    public static string Hash(Stream stream, MD5Format format = MD5Format.LowerNoHyphen)
    {
        using var md5 = SysMD5.Create();
        byte[] result = md5.ComputeHash(stream);
        return HashToString(result, format);
    }

    private static string HashToString(byte[] hash, MD5Format format)
    {
        var str = BitConverter.ToString(hash);
        if (format == MD5Format.LowerNoHyphen || format == MD5Format.UpperNoHyphen)
            str = str.Replace("-", string.Empty);
        if (format == MD5Format.LowerNoHyphen || format == MD5Format.LowerWithHyphen)
            str = str.ToLower();
        else if (format == MD5Format.UpperNoHyphen || format == MD5Format.UpperWithHyphen)
            str = str.ToUpper();
        return str;
    }
}
