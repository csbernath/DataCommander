using System;

namespace Foundation.Linq;

public static class TimeSpanExtensions
{
    public static double GetTotalMicroseconds(this TimeSpan timeSpan) => timeSpan.Ticks * 0.1;
}