using System;

namespace Foundation.Core.ClockAggregate;

internal sealed class ClockAggregateState(long stopwatchTimestamp, int environmentTickCount, DateTime universalTime, DateTime localTime)
{
    public readonly long StopwatchTimestamp = stopwatchTimestamp;
    public readonly int EnvironmentTickCount = environmentTickCount;
    public readonly DateTime UniversalTime = universalTime;
    public readonly DateTime LocalTime = localTime;
}