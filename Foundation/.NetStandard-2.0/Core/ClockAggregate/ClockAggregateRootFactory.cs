using System;
using System.Diagnostics;

namespace Foundation.Core.ClockAggregate
{
    public static class ClockAggregateRootFactory
    {
        public static ClockAggregateRoot Now()
        {
            var stopwatchTimestamp = Stopwatch.GetTimestamp();
            var environmentTickCount = Environment.TickCount;
            var universalTime = DateTime.UtcNow;
            var localTime = universalTime.ToLocalTime();
            var clockAggregateState = new ClockAggregateState(stopwatchTimestamp, environmentTickCount, universalTime, localTime);
            return new ClockAggregateRoot(clockAggregateState);
        }
    }
}