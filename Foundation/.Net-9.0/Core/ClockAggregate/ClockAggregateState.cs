using System;

namespace Foundation.Core.ClockAggregate;

internal sealed class ClockAggregateState(long stopwatchTimestamp, long environmentTickCount64, DateTime universalTime, DateTime localTime)
{
    public readonly long StopwatchTimestamp = stopwatchTimestamp;
    public readonly long EnvironmentTickCount64 = environmentTickCount64;
    public readonly DateTime UniversalTime = universalTime;
    public readonly DateTime LocalTime = localTime;
}