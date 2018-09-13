using System;
using System.Diagnostics;
using System.Text;

namespace Foundation.Core
{
    public struct StopwatchTimeSpan
    {
        #region Public Fields

        /// <summary>
        /// 
        /// </summary>
        public static readonly double TicksPerTick = (double) TimeSpan.TicksPerSecond / Stopwatch.Frequency;

        /// <summary>
        /// 
        /// </summary>
        public static readonly double TicksPerMicrosecond = Stopwatch.Frequency / 1000000.0;

        /// <summary>
        /// 
        /// </summary>
        public static readonly double TicksPerMillisecond = Stopwatch.Frequency / 1000.0;

        /// <summary>
        /// 
        /// </summary>
        public static readonly long TicksPerSecond = Stopwatch.Frequency;

        /// <summary>
        /// 
        /// </summary>
        public static readonly long TicksPerMinute = 60 * TicksPerSecond;

        /// <summary>
        /// 
        /// </summary>
        public static readonly long TicksPerHour = 60 * TicksPerMinute;

        /// <summary>
        /// 
        /// </summary>
        public static readonly long TicksPerDay = 24 * TicksPerHour;

        #endregion

        public StopwatchTimeSpan(long ticks) => Ticks = ticks;

        public StopwatchTimeSpan(TimeSpan timeSpan) => Ticks = ToTicks(timeSpan);

        public long Ticks { get; }

        public TimeSpan Elapsed => ToTimeSpan(Ticks);
        public double TotalHours => (double) Ticks / TicksPerHour;
        public double TotalMinutes => (double) Ticks / TicksPerMinute;
        public double TotalSeconds => (double) Ticks / TicksPerSecond;
        public double TotalMilliseconds => (double) Ticks * 1000 / TicksPerSecond;
        public double TotalMicroseconds => (double) Ticks * 1000000 / TicksPerSecond;
        public double TotalNanoseconds => (double) Ticks * 1000000000 / TicksPerSecond;

        public static int ToInt32(long ticks, int multiplier)
        {
            var d = (double) multiplier * ticks / TicksPerSecond;
            var int32 = (int) Math.Round(d);
            return int32;
        }

        public static long ToInt64(long ticks, long multiplier)
        {
            var d = (double) multiplier * ticks / Stopwatch.Frequency;
            var int64 = (long) Math.Round(d);
            return int64;
        }

        private static readonly long[] Power10 = new[]
        {
            1,
            10,
            100,
            1000,
            10000,
            100000,
            1000000,
            10000000,
            100000000,
            1000000000,
            10000000000,
            100000000000,
            1000000000000,
            10000000000000,
            100000000000000,
            1000000000000000
        };

        private static long Pow10(int pow) => Power10[pow];

        public static string ToString(long ticks, int scale)
        {
            var totalSeconds = ticks / TicksPerSecond;
            string fractionString = null;

            if (scale > 0)
            {
                var fractionTicks = ticks - (totalSeconds * TicksPerSecond);
                var multiplier = Pow10(scale);
                var fraction = (double) multiplier * fractionTicks / TicksPerSecond;
                fraction = Math.Round(fraction);
                var fractionInt64 = (long) fraction;
                if (fractionInt64 == multiplier)
                {
                    fractionInt64 = 0;
                    totalSeconds++;

                }

                fractionString = $".{fractionInt64.ToString().PadLeft(scale, '0')}";
            }

            var stringBuilder = new StringBuilder();
            var days = ticks / TicksPerDay;

            if (days != 0)
            {
                totalSeconds -= DateTimeConstants.SecondsPerDay * days;

                stringBuilder.Append(days);
                stringBuilder.Append('.');
            }

            var hours = totalSeconds / 3600;
            var seconds = (int) (totalSeconds - (hours * 3600));
            var minutes = seconds / 60;
            seconds -= minutes * 60;

            if (stringBuilder.Length > 0 || hours > 0)
            {
                stringBuilder.Append(hours.ToString().PadLeft(2, '0'));
                stringBuilder.Append(':');
            }

            stringBuilder.Append(minutes.ToString().PadLeft(2, '0'));
            stringBuilder.Append(':');
            stringBuilder.Append(seconds.ToString().PadLeft(2, '0'));

            var s = $"{stringBuilder}{fractionString}";
            return s;
        }

        public static long ToTicks(TimeSpan timeSpan) => (long) (timeSpan.TotalSeconds * Stopwatch.Frequency);

        public static TimeSpan ToTimeSpan(long elapsed)
        {
            var ticks = (long) (elapsed * TicksPerTick);
            return new TimeSpan(ticks);
        }

        public string ToString(int scale) => ToString(Ticks, scale);
        public override string ToString() => ToString(Ticks, 9);
    }
}