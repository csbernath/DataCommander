using System;
using System.Diagnostics;
using System.Text;

namespace Foundation.Diagnostics;

public static class StackTraceExtensions
{
    public static string ToLogString(this StackTrace trace)
    {
        ArgumentNullException.ThrowIfNull(trace);

        StringBuilder stringBuilder = new StringBuilder();
        int count = trace.FrameCount;
        for (int i = 0; i < count; ++i)
        {
            StackFrame frame = trace.GetFrame(i);
            stringBuilder.AppendLine(frame.ToLogString());
        }

        return stringBuilder.ToString();
    }

    public static string GetTrace(int skipFrames)
    {
        StackTrace trace = new StackTrace(skipFrames, true);
        return ToLogString(trace);
    }
}