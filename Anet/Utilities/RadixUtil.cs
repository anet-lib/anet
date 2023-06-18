namespace Anet.Utilities;

public class RadixUtil
{
    public const string BASE64 = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ-_";

    public static string Encode(long number, int radix)
    {
        var stack = new Stack<char>();
        while (number >= radix)
        {
            var remainder = (byte)(number % radix);
            stack.Push(BASE64[remainder]);
            number /= radix;
        }
        stack.Push(BASE64[(byte)number]);
        return new string(stack.ToArray());
    }

    public static long Decode(string text, int radix)
    {
        long result = 0;
        for (int i = 0; i < text.Length; i++)
        {
            result += (long)Math.Pow(radix, text.Length - i - 1) * BASE64.IndexOf(text[i]);
        }
        return result;
    }
}
