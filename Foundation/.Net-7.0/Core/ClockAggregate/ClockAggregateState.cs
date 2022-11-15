using System;

namespace Foundation.Core.ClockAggregate;

internal sealed class ClockAggregateState
{
    public readonly long StopwatchTimestamp;
    public readonly int EnvironmentTickCount;
    public readonly DateTime UniversalTime;
    public readonly DateTime LocalTime;

    public ClockAggregateState(long stopwatchTimestamp, int environmentTickCount, DateTime universalTime, DateTime localTime)
    {
        StopwatchTimestamp = stopwatchTimestamp;
        EnvironmentTickCount = environmentTickCount;
        UniversalTime = universalTime;
        LocalTime = localTime;
    }
}