using System;
using System.Diagnostics;

namespace Foundation.Core;

public static class StopwatchConstants
{
    public static readonly double TicksPerNanosecond = Stopwatch.Frequency / (double) PowersOf10.Power9;
    public static readonly double TicksPerMicrosecond = Stopwatch.Frequency / (double) PowersOf10.Power6;
    public static readonly double TicksPerMillisecond = Stopwatch.Frequency / (double) PowersOf10.Power3;
    public static readonly long TicksPerSecond = Stopwatch.Frequency;
    public static readonly long TicksPerMinute = TimeSpan.SecondsPerMinute * TicksPerSecond;
    public static readonly long TicksPerHour = TimeSpan.MinutesPerHour * TicksPerMinute;
    public static readonly long TicksPerDay = TimeSpan.HoursPerDay * TicksPerHour;
    public static readonly long TicksPerWeek = DateTimeConstants.DaysPerWeek * TicksPerDay;

    public static readonly double NanosecondsPerTick = (double) PowersOf10.Power9 / Stopwatch.Frequency;
    public static readonly double TimeSpanTicksPerStopwatchTick = (double) TimeSpan.TicksPerSecond / Stopwatch.Frequency;
    public static readonly double MicrosecondsPerTick = (double) PowersOf10.Power6 / Stopwatch.Frequency;
    public static readonly double MillisecondsPerTick = (double) PowersOf10.Power3 / Stopwatch.Frequency;
    public static readonly double SecondsPerTick = 1.0 / Stopwatch.Frequency;
}