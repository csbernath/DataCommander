using System;
using Foundation.Assertions;

namespace Foundation.Core.ClockAggregate;

public static class ClockAggregateRootQueries
{
    public static void Get(this ClockAggregateRoot clock, out long stopwatchTimestamp, out long environmentTickCount, out DateTime universalTime,
        out DateTime localTime)
    {
        var clockState = clock.GetAggregateState();
        stopwatchTimestamp = clockState.StopwatchTimestamp;
        environmentTickCount = clockState.EnvironmentTickCount64;
        universalTime = clockState.UniversalTime;
        localTime = clockState.LocalTime;
    }

    public static DateTime GetLocalTimeFromCurrentEnvironmentTickCount64(this ClockAggregateRoot clock)
    {
        var universalTime = clock.GetUniversalTimeFromCurrentEnvironmentTickCount64();
        var localTime = universalTime.ToLocalTime();
        return localTime;
    }

    public static DateTime GetUniversalTimeFromCurrentEnvironmentTickCount64(this ClockAggregateRoot clock) =>
        clock.GetUniversalTimeFromEnvironmentTickCount64(Environment.TickCount64);

    public static DateTime GetUniversalTimeFromStopwatchTimestamp(this ClockAggregateRoot clock, long stopwatchTimestamp)
    {
        Assert.IsNotNull(clock);
        var clockState = clock.GetAggregateState();
        var stopwatchTicks = stopwatchTimestamp - clockState.StopwatchTimestamp;
        var timeSpanTicksDouble = stopwatchTicks * StopwatchConstants.TimeSpanTicksPerStopwatchTick;
        var timeSpanTicks = (long) Math.Round(timeSpanTicksDouble);
        var universalTime = clockState.UniversalTime.AddTicks(timeSpanTicks);
        return universalTime;
    }

    public static (long environmentTickCount64, DateTime universalTime) GetFromStopwatchTimestamp(
        this ClockAggregateRoot clock,
        long stopwatchTimestamp)
    {
        Assert.IsNotNull(clock);
        var clockState = clock.GetAggregateState();
        var stopwatchTicks = stopwatchTimestamp - clockState.StopwatchTimestamp;

        var environmentTicksDouble = stopwatchTicks * StopwatchConstants.MillisecondsPerTick;
        var environmentTicks = (long)Math.Round(environmentTicksDouble);
        var environmentTickCount64 = clockState.EnvironmentTickCount64 + environmentTicks;

        var timeSpanTicksDouble = stopwatchTicks * StopwatchConstants.TimeSpanTicksPerStopwatchTick;
        var timeSpanTicks = (long)Math.Round(timeSpanTicksDouble);
        var universalTime = clockState.UniversalTime.AddTicks(timeSpanTicks);

        return (environmentTickCount64, universalTime);
    }

    private static DateTime GetUniversalTimeFromEnvironmentTickCount64(this ClockAggregateRoot clock, long environmentTickCount64)
    {
        Assert.IsNotNull(clock);
        var clockState = clock.GetAggregateState();
        var milliseconds = environmentTickCount64 - clockState.EnvironmentTickCount64;
        var universalTime = clockState.UniversalTime.AddMilliseconds(milliseconds);
        return universalTime;
    }
}