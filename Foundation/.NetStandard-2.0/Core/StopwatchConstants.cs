using System;
using System.Diagnostics;

namespace Foundation.Core
{
    public static class StopwatchConstants
    {
        public static readonly double TicksPerNanosecond = Stopwatch.Frequency / 1000000000.0;
        public static readonly double TicksPerMicrosecond = Stopwatch.Frequency / 1000000.0;
        public static readonly double TicksPerMillisecond = Stopwatch.Frequency / 1000.0;
        public static readonly long TicksPerSecond = Stopwatch.Frequency;
        public static readonly long TicksPerMinute = DateTimeConstants.SecondsPerMinute * TicksPerSecond;
        public static readonly long TicksPerHour = DateTimeConstants.MinutesPerHour * TicksPerMinute;
        public static readonly long TicksPerDay = DateTimeConstants.HoursPerDay * TicksPerHour;

        public static readonly double NanosecondsPerTick = 1000000000.0 / Stopwatch.Frequency;
        public static readonly double DateTimeTicksPerStopwatchTick = (double) TimeSpan.TicksPerSecond / Stopwatch.Frequency;
        public static readonly double MicrosecondsPerTick = 1000000.0 / Stopwatch.Frequency;
        public static readonly double MillisecondsPerTick = 1000.0 / Stopwatch.Frequency;
        public static readonly double SecondsPerTick = 1.0 / Stopwatch.Frequency;
    }
}