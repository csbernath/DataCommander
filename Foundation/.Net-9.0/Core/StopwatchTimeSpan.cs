using System;
using System.Diagnostics;
using System.Text;

namespace Foundation.Core;

public struct StopwatchTimeSpan(long ticks)
{
    private static readonly long[] Power10 =
    [
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
    ];

    public StopwatchTimeSpan(TimeSpan timeSpan) : this(ToTicks(timeSpan))
    {
    }

    public readonly long Ticks => ticks;
    public TimeSpan Elapsed => ToTimeSpan(Ticks);
    public double TotalHours => (double) Ticks / StopwatchConstants.TicksPerHour;
    public double TotalMinutes => (double) Ticks / StopwatchConstants.TicksPerMinute;
    public double TotalSeconds => (double) Ticks / StopwatchConstants.TicksPerSecond;
    public double TotalMilliseconds => Ticks * StopwatchConstants.TicksPerMillisecond;
    public double TotalMicroseconds => Ticks * StopwatchConstants.TicksPerMicrosecond;
    public double TotalNanoseconds => Ticks * StopwatchConstants.TicksPerNanosecond;

    public static int ToInt32(long ticks, int multiplier)
    {
        double d = (double) multiplier * ticks / StopwatchConstants.TicksPerSecond;
        int int32 = (int) Math.Round(d);
        return int32;
    }

    public static long ToInt64(long ticks, long multiplier)
    {
        double d = (double) multiplier * ticks / Stopwatch.Frequency;
        long int64 = (long) Math.Round(d);
        return int64;
    }

    public static string ToString(long ticks, int scale)
    {
        long totalSeconds = ticks / StopwatchConstants.TicksPerSecond;
        long fractionTicks = ticks - totalSeconds * StopwatchConstants.TicksPerSecond;
        long multiplier = Pow10(scale);
        double fraction = (double) multiplier * fractionTicks / StopwatchConstants.TicksPerSecond;
        fraction = Math.Round(fraction);
        long fractionInt64 = (long) fraction;
        if (fractionInt64 == multiplier)
        {
            fractionInt64 = 0;
            totalSeconds++;
        }

        string fractionString = null;
        if (scale > 0)
            fractionString = $".{fractionInt64.ToString().PadLeft(scale, '0')}";

        StringBuilder stringBuilder = new StringBuilder();
        long days = ticks / StopwatchConstants.TicksPerDay;

        if (days != 0)
        {
            totalSeconds -= TimeSpan.SecondsPerDay * days;

            stringBuilder.Append(days);
            stringBuilder.Append('.');
        }

        long hours = totalSeconds / 3600;
        int seconds = (int) (totalSeconds - (hours * 3600));
        int minutes = seconds / 60;
        seconds -= minutes * 60;

        if (stringBuilder.Length > 0 || hours > 0)
        {
            stringBuilder.Append(hours.ToString().PadLeft(2, '0'));
            stringBuilder.Append(':');
        }

        stringBuilder.Append(minutes.ToString().PadLeft(2, '0'));
        stringBuilder.Append(':');
        stringBuilder.Append(seconds.ToString().PadLeft(2, '0'));

        string s = $"{stringBuilder}{fractionString}";
        return s;
    }

    public static long ToTicks(TimeSpan timeSpan) => (long) (timeSpan.TotalSeconds * Stopwatch.Frequency);

    public static TimeSpan ToTimeSpan(long stopwatchTicks)
    {
        long dateTimeTicks = (long) (stopwatchTicks * StopwatchConstants.TimeSpanTicksPerStopwatchTick);
        return new TimeSpan(dateTimeTicks);
    }

    public string ToString(int scale) => ToString(Ticks, scale);
    public override string ToString() => ToString(Ticks, 9);
    private static long Pow10(int pow) => Power10[pow];
}