using System;
using System.Diagnostics;

namespace Foundation.Core.ClockAggregate;

public static class ClockAggregateRootFactory
{
    public static ClockAggregateRoot Now()
    {
        var stopwatchTimestamp = Stopwatch.GetTimestamp();
        var environmentTickCount = Environment.TickCount64;
        var universalDateTimeOffset = DateTimeOffset.UtcNow;
        var localDateTimeOffset = universalDateTimeOffset.ToLocalTime();
        var clockAggregateState = new ClockAggregateState(stopwatchTimestamp, environmentTickCount, universalDateTimeOffset, localDateTimeOffset);
        return new ClockAggregateRoot(clockAggregateState);
    }
}