using System;

namespace Foundation.Core;

public struct NanoTimestamp
{
    private readonly ulong _ticks;

    private NanoTimestamp(ulong ticks) => _ticks = ticks;

    public DateTime ToUniversalTime()
    {
        var timeSpanTicks = (long) (_ticks / 100);
        var universalTime = new DateTime(timeSpanTicks);
        return universalTime;
    }

    public static NanoTimestamp FromStopwatchTimestamp(DateTime referenceUniversalTime, long referenceStopwatchTimestamp, long stopwatchTimestamp)
    {
        var referenceNanoTimestampTicksDouble = referenceUniversalTime.Ticks * 100.0;
        var stopwatchTicks = stopwatchTimestamp - referenceStopwatchTimestamp;
        var seconds = (double) stopwatchTicks / StopwatchConstants.TicksPerSecond;
        var nanoseconds = seconds * TenPowerConstants.TenPower9;
        var nanoTimestampTicksDouble = referenceNanoTimestampTicksDouble + nanoseconds;
        var nanoTimestampTicks = (ulong) nanoTimestampTicksDouble;
        return new NanoTimestamp(nanoTimestampTicks);
    }
}