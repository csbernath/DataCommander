using System;
using System.Diagnostics;

namespace Foundation.Core.ClockAggregate
{
    public static class ClockAggregateRootFactory
    {
        public static ClockAggregateRoot Now()
        {
            var utcDateTime = DateTime.UtcNow;
            var environmentTickCount = Environment.TickCount;
            var stopwatchTimestamp = Stopwatch.GetTimestamp();
            return new ClockAggregateRoot(new ClockAggregateState(utcDateTime, environmentTickCount, stopwatchTimestamp));
        }
    }
}