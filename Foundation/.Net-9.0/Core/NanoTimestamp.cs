using System;

namespace Foundation.Core;

public readonly struct NanoTimestamp
{
    private readonly ulong _ticks;

    private NanoTimestamp(ulong ticks) => _ticks = ticks;

    public readonly DateTime ToUniversalTime()
    {
        long timeSpanTicks = (long) (_ticks / 100);
        DateTime universalTime = new DateTime(timeSpanTicks);
        return universalTime;
    }

    public static NanoTimestamp FromStopwatchTimestamp(DateTime referenceUniversalTime, long referenceStopwatchTimestamp, long stopwatchTimestamp)
    {
        double referenceNanoTimestampTicksDouble = referenceUniversalTime.Ticks * 100.0;
        long stopwatchTicks = stopwatchTimestamp - referenceStopwatchTimestamp;
        double seconds = (double) stopwatchTicks / StopwatchConstants.TicksPerSecond;
        double nanoseconds = seconds * TenPowerConstants.TenPower9;
        double nanoTimestampTicksDouble = referenceNanoTimestampTicksDouble + nanoseconds;
        ulong nanoTimestampTicks = (ulong) nanoTimestampTicksDouble;
        return new NanoTimestamp(nanoTimestampTicks);
    }
}