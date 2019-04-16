using System;
using Foundation.Assertions;

namespace Foundation.Core.ClockAggregate
{
    public static class ClockAggregateRootQueries
    {
        public static DateTime GetLocalTimeFromCurrentEnvironmentTickCount(this ClockAggregateRoot clock)
        {
            var universalTime = clock.GetUniversalTimeFromCurrentEnvironmentTickCount();
            var localTime = universalTime.ToLocalTime();
            return localTime;
        }

        public static DateTime GetUniversalTimeFromCurrentEnvironmentTickCount(this ClockAggregateRoot clock) =>
            clock.GetUniversalTimeFromEnvironmentTickCount(Environment.TickCount);

        //public static DateTime GetUniversalTimeFromStopwatchTimestamp(this ClockAggregateRoot clock, long stopwatchTimestamp)
        //{
        //    Assert.IsNotNull(clock);
        //    var clockState = clock.GetAggregateState();
        //    var stopwatchTicks = stopwatchTimestamp - clockState.StopwatchTimestamp;
        //    var dateTimeTicksDouble = stopwatchTicks * StopwatchConstants.DateTimeTicksPerStopwatchTick;
        //    var dateTimeTicksLong = (long) Math.Round(dateTimeTicksDouble);
        //    var utcDateTime = clockState.UniversalTime.AddTicks(dateTimeTicksLong);
        //    return utcDateTime;
        //}

        private static DateTime GetUniversalTimeFromEnvironmentTickCount(this ClockAggregateRoot clock, int environmentTickCount)
        {
            Assert.IsNotNull(clock);
            var clockState = clock.GetAggregateState();
            var milliseconds = environmentTickCount - clockState.EnvironmentTickCount;
            var universalTime = clockState.UniversalTime.AddMilliseconds(milliseconds);
            return universalTime;
        }
    }
}