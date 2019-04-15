using System;

namespace Foundation.Core.ClockAggregate
{
    public class ClockAggregateState
    {
        public readonly DateTime UtcDateTime;
        public readonly int EnvironmentTickCount;
        public readonly long StopwatchTimestamp;

        public ClockAggregateState(DateTime utcDateTime, int environmentTickCount, long stopwatchTimestamp)
        {
            UtcDateTime = utcDateTime;
            EnvironmentTickCount = environmentTickCount;
            StopwatchTimestamp = stopwatchTimestamp;
        }
    }
}