using System;

namespace Foundation.Core.ClockAggregate;

public static class ClockAggregateRootQueries
{
    public static void Get(this ClockAggregateRoot clock, out long stopwatchTimestamp, out long environmentTickCount, out DateTime universalTime,
        out DateTime localTime)
    {
        ClockAggregateState clockState = clock.GetAggregateState();
        stopwatchTimestamp = clockState.StopwatchTimestamp;
        environmentTickCount = clockState.EnvironmentTickCount64;
        universalTime = clockState.UniversalTime;
        localTime = clockState.LocalTime;
    }

    public static DateTime GetLocalTimeFromCurrentEnvironmentTickCount(this ClockAggregateRoot clock)
    {
        DateTime universalTime = clock.GetUniversalTimeFromCurrentEnvironmentTickCount();
        DateTime localTime = universalTime.ToLocalTime();
        return localTime;
    }

    public static DateTime GetUniversalTimeFromCurrentEnvironmentTickCount(this ClockAggregateRoot clock) =>
        clock.GetUniversalTimeFromEnvironmentTickCount64(Environment.TickCount64);

    public static DateTime GetUniversalTimeFromStopwatchTimestamp(this ClockAggregateRoot clock, long stopwatchTimestamp)
    {
        ArgumentNullException.ThrowIfNull(clock);
        ClockAggregateState clockState = clock.GetAggregateState();
        long stopwatchTicks = stopwatchTimestamp - clockState.StopwatchTimestamp;
        double timeSpanTicksDouble = stopwatchTicks * StopwatchConstants.TimeSpanTicksPerStopwatchTick;
        long timeSpanTicks = (long) Math.Round(timeSpanTicksDouble);
        DateTime universalTime = clockState.UniversalTime.AddTicks(timeSpanTicks);
        return universalTime;
    }

    public static (long environmentTickCount64, DateTime universalTime) GetFromStopwatchTimestamp(
        this ClockAggregateRoot clock,
        long stopwatchTimestamp)
    {
        ArgumentNullException.ThrowIfNull(clock);
        ClockAggregateState clockState = clock.GetAggregateState();
        long stopwatchTicks = stopwatchTimestamp - clockState.StopwatchTimestamp;

        double environmentTicksDouble = stopwatchTicks * StopwatchConstants.MillisecondsPerTick;
        long environmentTicks = (long)Math.Round(environmentTicksDouble);
        long environmentTickCount64 = clockState.EnvironmentTickCount64 + environmentTicks;

        double timeSpanTicksDouble = stopwatchTicks * StopwatchConstants.TimeSpanTicksPerStopwatchTick;
        long timeSpanTicks = (long)Math.Round(timeSpanTicksDouble);
        DateTime universalTime = clockState.UniversalTime.AddTicks(timeSpanTicks);

        return (environmentTickCount64, universalTime);
    }

    private static DateTime GetUniversalTimeFromEnvironmentTickCount64(this ClockAggregateRoot clock, long environmentTickCount64)
    {
        ArgumentNullException.ThrowIfNull(clock);
        ClockAggregateState clockState = clock.GetAggregateState();
        long milliseconds = environmentTickCount64 - clockState.EnvironmentTickCount64;
        DateTime universalTime = clockState.UniversalTime.AddMilliseconds(milliseconds);
        return universalTime;
    }
}