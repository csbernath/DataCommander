using System;
using System.Diagnostics;
using System.Text;

namespace Foundation.Diagnostics;

public static class StackTraceExtensions
{
    public static string ToLogString(this StackTrace trace)
    {
        ArgumentNullException.ThrowIfNull(trace);

        var stringBuilder = new StringBuilder();
        var count = trace.FrameCount;
        for (var i = 0; i < count; ++i)
        {
            var frame = trace.GetFrame(i)!;
            stringBuilder.AppendLine(frame.ToLogString());
        }

        return stringBuilder.ToString();
    }

    public static string GetTrace(int skipFrames)
    {
        var trace = new StackTrace(skipFrames, true);
        return ToLogString(trace);
    }
}