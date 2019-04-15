using System;
using Foundation.Assertions;

namespace Foundation.Core.ClockAggregate
{
    public class ClockAggregateRoot
    {
        private readonly ClockAggregateState _clockAggregateState;

        public ClockAggregateRoot(ClockAggregateState clockAggregateState)
        {
            Assert.IsNotNull(clockAggregateState);
            _clockAggregateState = clockAggregateState;
        }

        public ClockAggregateState GetAggregateState() => _clockAggregateState;

        public DateTime GetUtcDateTimeFromEnvironmentTickCount(int environmentTickCount)
        {
            var clockState = _clockAggregateState;
            var milliseconds = environmentTickCount - clockState.EnvironmentTickCount;
            var utcDateTime = clockState.UtcDateTime.AddMilliseconds(milliseconds);
            return utcDateTime;
        }

        public DateTime GetUtcDateTimeFromStopwatchTimestamp(long stopwatchTimestamp)
        {
            var clockState = _clockAggregateState;
            var stopwatchTicks = stopwatchTimestamp - clockState.StopwatchTimestamp;
            var dateTimeTicksDouble = stopwatchTicks * StopwatchConstants.DateTimeTicksPerStopwatchTick;
            var dateTimeTicksLong = (long) Math.Round(dateTimeTicksDouble);
            var utcDateTime = clockState.UtcDateTime.AddTicks(dateTimeTicksLong);
            return utcDateTime;
        }
    }
}