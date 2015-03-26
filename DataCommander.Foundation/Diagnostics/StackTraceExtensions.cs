namespace DataCommander.Foundation.Diagnostics
{
    using System;
    using System.Diagnostics;
    using System.Diagnostics.Contracts;
    using System.Text;

    internal static class StackTraceExtensions
    {
        private static readonly ILog log = InternalLogFactory.Instance.GetCurrentTypeLog();

        public static string ToLogString(this StackTrace trace)
        {
            Contract.Requires(trace != null);

            var sb = new StringBuilder();

            try
            {
                int count = trace.FrameCount;

                for (int i = 0; i < count; i++)
                {
                    StackFrame frame = trace.GetFrame(i);
                    sb.Append(frame.ToLogString());
                    sb.Append(Environment.NewLine);
                }
            }
            catch (Exception e)
            {
                log.Write(LogLevel.Error, e.ToString());
            }

            return sb.ToString();
        }

        public static string GetTrace(int skipFrames)
        {
            var trace = new StackTrace(skipFrames, true);
            return ToLogString(trace);
        }
    }
}