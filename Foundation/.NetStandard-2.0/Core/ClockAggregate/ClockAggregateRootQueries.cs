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

        public static DateTime GetUniversalTimeFromStopwatchTimestamp(this ClockAggregateRoot clock, long stopwatchTimestamp)
        {
            Assert.IsNotNull(clock);
            var clockState = clock.GetAggregateState();
            var stopwatchTicks = stopwatchTimestamp - clockState.StopwatchTimestamp;
            var timeSpanTicksDouble = stopwatchTicks * StopwatchConstants.TimeSpanTicksPerStopwatchTick;
            var timeSpanTicks = (long) Math.Round(timeSpanTicksDouble);
            var universalTime = clockState.UniversalTime.AddTicks(timeSpanTicks);
            return universalTime;
        }

        public static void GetFromStopwatchTimestamp(this ClockAggregateRoot clock, long stopwatchTimestamp, out int environmentTickCount, out DateTime universalTime)
        {
            Assert.IsNotNull(clock);
            var clockState = clock.GetAggregateState();
            var stopwatchTicks = stopwatchTimestamp - clockState.StopwatchTimestamp;

            var environmentTicksDouble = stopwatchTicks * StopwatchConstants.MillisecondsPerTick;
            var environmentTicks = (int) Math.Round(environmentTicksDouble);
            environmentTickCount = clockState.EnvironmentTickCount + environmentTicks;

            var timeSpanTicksDouble = stopwatchTicks * StopwatchConstants.TimeSpanTicksPerStopwatchTick;
            var timeSpanTicks = (long) Math.Round(timeSpanTicksDouble);
            universalTime = clockState.UniversalTime.AddTicks(timeSpanTicks);
        }

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