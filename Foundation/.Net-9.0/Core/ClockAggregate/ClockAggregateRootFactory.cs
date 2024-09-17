using System;
using System.Diagnostics;

namespace Foundation.Core.ClockAggregate;

public static class ClockAggregateRootFactory
{
    public static ClockAggregateRoot Now()
    {
        long stopwatchTimestamp = Stopwatch.GetTimestamp();
        long environmentTickCount = Environment.TickCount64;
        DateTime universalTime = DateTime.UtcNow;
        DateTime localTime = universalTime.ToLocalTime();
        ClockAggregateState clockAggregateState = new ClockAggregateState(stopwatchTimestamp, environmentTickCount, universalTime, localTime);
        return new ClockAggregateRoot(clockAggregateState);
    }
}