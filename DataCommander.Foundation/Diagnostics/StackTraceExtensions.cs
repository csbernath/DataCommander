namespace DataCommander.Foundation.Diagnostics
{
    using System;
    using System.Diagnostics;
    using System.Text;
    using DataCommander.Foundation.Diagnostics.Log;

    /// <summary>
    /// 
    /// </summary>
    public static class StackTraceExtensions
    {
        private static readonly ILog log = InternalLogFactory.Instance.GetCurrentTypeLog();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="trace"></param>
        /// <returns></returns>
        public static string ToLogString(this StackTrace trace)
        {
#if CONTRACTS_FULL
            Contract.Requires<ArgumentNullException>(trace != null);
#endif

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
                log.Write(LogLevel.Error, e.ToString());
            }

            return stringBuilder.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="skipFrames"></param>
        /// <returns></returns>
        public static string GetTrace(int skipFrames)
        {
            var trace = new StackTrace(skipFrames, true);
            return ToLogString(trace);
        }
    }
}