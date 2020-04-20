namespace System
{
    public static class DateTimeExtensions
    {
        public static long ToTimestamp(this DateTime time)
        {
            return ((DateTimeOffset)time).ToUnixTimeSeconds();
        }

        public static string ToWeekCN(this DateTime date, string todayStr = "今天")
        {
            if (!string.IsNullOrEmpty(todayStr) && DateTime.Today == date)
                return todayStr;
            return date.DayOfWeek switch
            {
                DayOfWeek.Monday => "周一",
                DayOfWeek.Tuesday => "周二",
                DayOfWeek.Wednesday => "周三",
                DayOfWeek.Thursday => "周四",
                DayOfWeek.Friday => "周五",
                DayOfWeek.Saturday => "周六",
                DayOfWeek.Sunday => "周日",
                _ => "",
            };
        }
    }
}
