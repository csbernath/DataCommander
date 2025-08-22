using System;

namespace Foundation.Core.ClockAggregate;

public static class ClockAggregateRootQueries
{
    extension(ClockAggregateRoot clock)
    {
        public DateTimeOffset GetLocalDateTimeOffsetFromCurrentEnvironmentTickCount64()
        {
            var universalTime = clock.GetUtcFromCurrentEnvironmentTickCount64();
            var localTime = universalTime.ToLocalTime();
            return localTime;
        }

        public DateTimeOffset GetUtcFromCurrentEnvironmentTickCount64() =>
            clock.GetUtcFromEnvironmentTickCount64(Environment.TickCount64);

        public DateTimeOffset GetUniversalDateTimeOffsetFromStopwatchTimestamp(long stopwatchTimestamp)
        {
            ArgumentNullException.ThrowIfNull(clock);
            var clockState = clock.GetAggregateState();
            var stopwatchTicks = stopwatchTimestamp - clockState.StopwatchTimestamp;
            var timeSpanTicksDouble = stopwatchTicks * StopwatchConstants.TimeSpanTicksPerStopwatchTick;
            var timeSpanTicks = (long)Math.Round(timeSpanTicksDouble);
            var universalTime = clockState.UniversalDateTimeOffset.AddTicks(timeSpanTicks);
            return universalTime;
        }

        public (long environmentTickCount64, DateTimeOffset universalDateTimeOffset) GetFromStopwatchTimestamp(
            long stopwatchTimestamp)
        {
            ArgumentNullException.ThrowIfNull(clock);
            var clockState = clock.GetAggregateState();
            var stopwatchTicks = stopwatchTimestamp - clockState.StopwatchTimestamp;

            var environmentTicksDouble = stopwatchTicks * StopwatchConstants.MillisecondsPerTick;
            var environmentTicks = (long)Math.Round(environmentTicksDouble);
            var environmentTickCount64 = clockState.EnvironmentTickCount64 + environmentTicks;

            var timeSpanTicksDouble = stopwatchTicks * StopwatchConstants.TimeSpanTicksPerStopwatchTick;
            var timeSpanTicks = (long)Math.Round(timeSpanTicksDouble);
            var universalDateTimeOffset = clockState.UniversalDateTimeOffset.AddTicks(timeSpanTicks);

            return (environmentTickCount64, universalDateTimeOffset);
        }

        public DateTimeOffset GetUtcFromEnvironmentTickCount64(long environmentTickCount64)
        {
            ArgumentNullException.ThrowIfNull(clock);
            var clockState = clock.GetAggregateState();
            var milliseconds = environmentTickCount64 - clockState.EnvironmentTickCount64;
            var universalTime = clockState.UniversalDateTimeOffset.AddMilliseconds(milliseconds);
            return universalTime;
        }
    }
}