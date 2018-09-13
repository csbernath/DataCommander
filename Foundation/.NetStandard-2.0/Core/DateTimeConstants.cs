namespace Foundation.Core
{
    public static class DateTimeConstants
    {
        public const int HoursPerDay = 24;
        public const int MillisecondsPerDay = MinutesPerDay * 1000;
        public const int MinutesPerDay = MinutesPerHour * HoursPerDay;
        public const int MinutesPerHour = 60;
        public const int SecondsPerHour = MinutesPerHour * SecondsPerMinute;
        public const int SecondsPerMinute = 60;
        public const int SecondsPerDay = SecondsPerMinute * MinutesPerDay;
        public const int DaysPerWeek = 7;
    }
}