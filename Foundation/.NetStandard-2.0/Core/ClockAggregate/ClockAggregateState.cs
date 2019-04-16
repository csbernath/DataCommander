using System;

namespace Foundation.Core.ClockAggregate
{
    internal sealed class ClockAggregateState
    {
        public readonly DateTime UniversalTime;
        public readonly int EnvironmentTickCount;
        public readonly long StopwatchTimestamp;

        public ClockAggregateState(DateTime universalTime, int environmentTickCount, long stopwatchTimestamp)
        {
            UniversalTime = universalTime;
            EnvironmentTickCount = environmentTickCount;
            StopwatchTimestamp = stopwatchTimestamp;
        }
    }
}