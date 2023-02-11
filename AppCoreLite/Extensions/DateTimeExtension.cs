namespace AppCoreLite.Extensions
{
    public static class DateTimeExtension
    {
        public static DateTime AddTime(this DateTime value, int hours = 0, int minutes = 0, int seconds = 0)
        {
            value = value.AddHours(hours);
            value = value.AddMinutes(minutes);
            value = value.AddSeconds(seconds);
            return value;
        }
    }
}
