namespace System;

public static class DateTimeExtensions
{
    public static long ToTimestamp(this DateTime time)
    {
        return ((DateTimeOffset)time).ToUnixTimeSeconds();
    }
}
