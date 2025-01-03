using System;

namespace Foundation.Core.ClockAggregate;

internal sealed class ClockAggregateState(
    long stopwatchTimestamp,
    long environmentTickCount64,
    DateTimeOffset universalDateTimeOffset,
    DateTimeOffset localDateTimeOffset)
{
    public readonly long StopwatchTimestamp = stopwatchTimestamp;
    public readonly long EnvironmentTickCount64 = environmentTickCount64;
    public readonly DateTimeOffset UniversalDateTimeOffset = universalDateTimeOffset;
    public readonly DateTimeOffset LocalDateTimeOffset = localDateTimeOffset;
}