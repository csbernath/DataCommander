using System;
using Foundation.Assertions;

namespace Foundation.Core.ClockAggregate;

public static class ClockAggregateRootQueries
{
    public static void Get(this ClockAggregateRoot clock, out long stopwatchTimestamp, out int environmentTickCount, out DateTime universalTime,
        out DateTime localTime)
    {
        var clockState = clock.GetAggregateState();
        stopwatchTimestamp = clockState.StopwatchTimestamp;
        environmentTickCount = clockState.EnvironmentTickCount;
        universalTime = clockState.UniversalTime;
        localTime = clockState.LocalTime;
    }

    public static DateTime GetLocalTimeFromCurrentEnvironmentTickCount(this ClockAggregateRoot clock)
    {
        var universalTime = clock.GetUniversalTimeFromCurrentEnvironmentTickCount();
        var localTime = universalTime.ToLocalTime();
        return localTime;
    }

    public static DateTime GetUniversalTimeFromCurrentEnvironmentTickCount(this ClockAggregateRoot clock) =>
        clock.GetUniversalTimeFromEnvironmentTickCount(Environment.TickCount);

    public static DateTime GetUniversalTimeFromStopwatchTimestamp(this ClockAggregateRoot clock, long stopwatchTimestamp)
    {
        ArgumentNullException.ThrowIfNull(clock);
        var clockState = clock.GetAggregateState();
        var stopwatchTicks = stopwatchTimestamp - clockState.StopwatchTimestamp;
        var timeSpanTicksDouble = stopwatchTicks * StopwatchConstants.TimeSpanTicksPerStopwatchTick;
        var timeSpanTicks = (long) Math.Round(timeSpanTicksDouble);
        var universalTime = clockState.UniversalTime.AddTicks(timeSpanTicks);
        return universalTime;
    }

    public static void GetFromStopwatchTimestamp(this ClockAggregateRoot clock, long stopwatchTimestamp, out int environmentTickCount,
        out DateTime universalTime)
    {
        ArgumentNullException.ThrowIfNull(clock);
        var clockState = clock.GetAggregateState();
        var stopwatchTicks = stopwatchTimestamp - clockState.StopwatchTimestamp;

        var environmentTicksDouble = stopwatchTicks * StopwatchConstants.MillisecondsPerTick;
        var environmentTicks = (int) Math.Round(environmentTicksDouble);
        environmentTickCount = clockState.EnvironmentTickCount + environmentTicks;

        var timeSpanTicksDouble = stopwatchTicks * StopwatchConstants.TimeSpanTicksPerStopwatchTick;
        var timeSpanTicks = (long) Math.Round(timeSpanTicksDouble);
        universalTime = clockState.UniversalTime.AddTicks(timeSpanTicks);
    }

    private static DateTime GetUniversalTimeFromEnvironmentTickCount(this ClockAggregateRoot clock, int environmentTickCount)
    {
        ArgumentNullException.ThrowIfNull(clock);
        var clockState = clock.GetAggregateState();
        var milliseconds = environmentTickCount - clockState.EnvironmentTickCount;
        var universalTime = clockState.UniversalTime.AddMilliseconds(milliseconds);
        return universalTime;
    }
}