using System;
using System.Diagnostics;

namespace Foundation.Core.Timers
{
    public static class ClockFactory
    {
        public static Clock CreateClock()
        {
            var utcDateTime = DateTime.UtcNow;
            var environmentTickCount = Environment.TickCount;
            var stopwatchTimestamp = Stopwatch.GetTimestamp();
            return new Clock(new ClockState(utcDateTime, environmentTickCount, stopwatchTimestamp));
        }
    }
}