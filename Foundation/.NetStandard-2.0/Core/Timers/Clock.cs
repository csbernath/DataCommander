using System;

namespace Foundation.Core.Timers
{
    public class Clock
    {
        private ClockState _clockState;
        public Clock(ClockState clockState) => _clockState = clockState;
        public void SetClockState(ClockState clockState) => _clockState = clockState;
        public ClockState GetClockState() => _clockState;

        public DateTime GetUtcDateTimeFromEnvironmentTickCount(int environmentTickCount)
        {
            var clockState = _clockState;
            var milliSeconds = environmentTickCount - clockState.EnvironmentTickCount;
            var utcDateTime = clockState.UtcDateTime.AddMilliseconds(milliSeconds);
            return utcDateTime;
        }

        public DateTime GetUtcDateTimeFromStopwatchTimestamp(long stopwatchTimestamp)
        {
            var clockState = _clockState;
            var stopwatchTicks = stopwatchTimestamp - clockState.StopwatchTimestamp;
            var dateTimeTicksDouble = stopwatchTicks * StopwatchTimeSpan.TicksPerTick;
            var dateTimeTicksLong = (long) Math.Round(dateTimeTicksDouble);
            var utcDateTime = clockState.UtcDateTime.AddTicks(dateTimeTicksLong);
            return utcDateTime;
        }
    }
}