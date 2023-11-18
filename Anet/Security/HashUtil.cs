using System.Text;

namespace Anet.Security;

public static class HashUtil
{
    public static string BytesToString(byte[] bytes)
    {
        var sb = new StringBuilder();
        foreach (byte b in bytes)
        {
            sb.Append(b.ToString("x2"));
        }
        return sb.ToString();
    }
}
