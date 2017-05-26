using System;
using System.Threading;

namespace Foundation.Diagnostics.Log
{
#if FOUNDATION_3_5
    using System.Web;
#endif

    internal static class LogEntryFactory
    {
        private static long _id;

        public static LogEntry Create(
            string logName,
            DateTime creationTime,
            string message,
            LogLevel logLevel)
        {
            var id = Interlocked.Increment(ref LogEntryFactory._id);
            var thread = Thread.CurrentThread;
            var threadId = thread.ManagedThreadId;
            var threadName = thread.Name;
            string userName = null;
            string hostName = null;

#if FOUNDATION_3_5
            HttpContext context = HttpContext.Current;

            if (context != null)
            {
                HttpRequest request;

                try
                {
                    request = context.Request;
                }
                catch
                {
                    request = null;
                }

                if (request != null)
                {
                    userName = request.ServerVariables["AUTH_USER"];

                    if (userName.Length == 0)
                    {
                        userName = null;
                    }

                    hostName = request.UserHostName;
                }
            }
#endif

            return new LogEntry(id, logName, creationTime, threadId, threadName, userName, hostName, message, logLevel);
        }
    }
}