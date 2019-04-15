using System;
using System.Collections.Generic;
using System.Text;

namespace Foundation.Core.Timers
{
    public class ClockState
    {
        public readonly DateTime UtcDateTime;
        public readonly int EnvironmentTickCount;
        public readonly long StopwatchTimestamp;

        public ClockState(DateTime utcDateTime, int environmentTickCount, long stopwatchTimestamp)
        {
            UtcDateTime = utcDateTime;
            EnvironmentTickCount = environmentTickCount;
            StopwatchTimestamp = stopwatchTimestamp;
        }
    }
}