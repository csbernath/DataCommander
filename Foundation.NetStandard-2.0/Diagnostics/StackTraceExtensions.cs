using System;
using System.Diagnostics;
using System.Text;
using Foundation.Assertions;
using Foundation.InternalLog;
using Foundation.Log;

namespace Foundation.Diagnostics
{
    public static class StackTraceExtensions
    {
        private static readonly ILog Log = InternalLogFactory.Instance.GetCurrentTypeLog();

        public static string ToLogString(this StackTrace trace)
        {
            Assert.IsNotNull(trace);

            var stringBuilder = new StringBuilder();

            try
            {
                var count = trace.FrameCount;

                for (var i = 0; i < count; ++i)
                {
                    var frame = trace.GetFrame(i);
                    stringBuilder.AppendLine(frame.ToLogString());
                }
            }
            catch (Exception e)
            {
                Log.Write(LogLevel.Error, e.ToString());
            }

            return stringBuilder.ToString();
        }

        public static string GetTrace(int skipFrames)
        {
            var trace = new StackTrace(skipFrames, true);
            return ToLogString(trace);
        }
    }
}