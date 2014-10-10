namespace DataCommander.Foundation.Diagnostics
{
    using System;
    using System.Diagnostics;
    using System.Diagnostics.Contracts;
    using System.Text;

    internal static class StackTraceExtensions
    {
        private static readonly ILog log = InternalLogFactory.Instance.GetCurrentTypeLog();

        public static String ToLogString(this StackTrace trace)
        {
            Contract.Requires(trace != null);

            var sb = new StringBuilder();

            try
            {
                Int32 count = trace.FrameCount;

                for (Int32 i = 0; i < count; i++)
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

        public static String GetTrace(Int32 skipFrames)
        {
            var trace = new StackTrace(skipFrames, true);
            return ToLogString(trace);
        }
    }
}