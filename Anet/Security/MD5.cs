﻿using System.Text;

using Hasher = System.Security.Cryptography.MD5;

namespace Anet.Security;

public static class MD5
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
        using var md5 = Hasher.Create();
        byte[] result = md5.ComputeHash(stream);
        return HashUtil.BytesToString(result);
    }
}
