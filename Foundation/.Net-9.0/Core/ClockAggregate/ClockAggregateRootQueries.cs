using System;
using Foundation.Assertions;

namespace Foundation.Core.ClockAggregate;

public static class ClockAggregateRootQueries
{
    public static DateTimeOffset GetUniversalTimeOffset(this ClockAggregateRoot clock)
    {
        var clockState = clock.GetAggregateState();
        return clockState.UniversalDateTimeOffset;
    }

    public static DateTimeOffset GetLocalDateTimeOffsetFromCurrentEnvironmentTickCount64(this ClockAggregateRoot clock)
    {
        var universalTime = clock.GetUniversalTimeFromCurrentEnvironmentTickCount64();
        var localTime = universalTime.ToLocalTime();
        return localTime;
    }

    public static DateTimeOffset GetUniversalTimeFromCurrentEnvironmentTickCount64(this ClockAggregateRoot clock) =>
        clock.GetUniversalDateTimeOffsetFromEnvironmentTickCount64(Environment.TickCount64);

    public static DateTimeOffset GetUniversalDateTimeOffsetFromStopwatchTimestamp(this ClockAggregateRoot clock, long stopwatchTimestamp)
    {
        Assert.IsNotNull(clock);
        var clockState = clock.GetAggregateState();
        var stopwatchTicks = stopwatchTimestamp - clockState.StopwatchTimestamp;
        var timeSpanTicksDouble = stopwatchTicks * StopwatchConstants.TimeSpanTicksPerStopwatchTick;
        var timeSpanTicks = (long)Math.Round(timeSpanTicksDouble);
        var universalTime = clockState.UniversalDateTimeOffset.AddTicks(timeSpanTicks);
        return universalTime;
    }

    public static (long environmentTickCount64, DateTimeOffset universalDateTimeOffset) GetFromStopwatchTimestamp(
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
        var universalDateTimeOffset = clockState.UniversalDateTimeOffset.AddTicks(timeSpanTicks);

        return (environmentTickCount64, universalDateTimeOffset);
    }

    private static DateTimeOffset GetUniversalDateTimeOffsetFromEnvironmentTickCount64(this ClockAggregateRoot clock, long environmentTickCount64)
    {
        Assert.IsNotNull(clock);
        var clockState = clock.GetAggregateState();
        var milliseconds = environmentTickCount64 - clockState.EnvironmentTickCount64;
        var universalTime = clockState.UniversalDateTimeOffset.AddMilliseconds(milliseconds);
        return universalTime;
    }
}