using System;
using System.Diagnostics;

namespace Foundation.Core;

public static class StopwatchConstants
{
    public static readonly double TicksPerNanosecond = Stopwatch.Frequency / (double) TenPowerConstants.TenPower9;
    public static readonly double TicksPerMicrosecond = Stopwatch.Frequency / (double) TenPowerConstants.TenPower6;
    public static readonly double TicksPerMillisecond = Stopwatch.Frequency / (double) TenPowerConstants.TenPower3;
    public static readonly long TicksPerSecond = Stopwatch.Frequency;
    public static readonly long TicksPerMinute = TimeSpan.SecondsPerMinute * TicksPerSecond;
    public static readonly long TicksPerHour = TimeSpan.MinutesPerHour * TicksPerMinute;
    public static readonly long TicksPerDay = TimeSpan.HoursPerDay * TicksPerHour;
    public static readonly long TicksPerWeek = DateTimeConstants.DaysPerWeek * TicksPerDay;

    public static readonly double NanosecondsPerTick = (double) TenPowerConstants.TenPower9 / Stopwatch.Frequency;
    public static readonly double TimeSpanTicksPerStopwatchTick = (double) TimeSpan.TicksPerSecond / Stopwatch.Frequency;
    public static readonly double MicrosecondsPerTick = (double) TenPowerConstants.TenPower6 / Stopwatch.Frequency;
    public static readonly double MillisecondsPerTick = (double) TenPowerConstants.TenPower3 / Stopwatch.Frequency;
    public static readonly double SecondsPerTick = 1.0 / Stopwatch.Frequency;
}