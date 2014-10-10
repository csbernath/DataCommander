namespace DataCommander.Foundation.Diagnostics
{
    using System;
    using System.Threading;
#if FOUNDATION_3_5
    using System.Web;
#endif

    internal static class LogEntryFactory
    {
        private static Int64 id;

        public static LogEntry Create(
            String logName,
            DateTime creationTime,
            String message,
            LogLevel logLevel )
        {
            Int64 id = Interlocked.Increment( ref LogEntryFactory.id );
            var thread = Thread.CurrentThread;
            Int32 threadId = thread.ManagedThreadId;
            String threadName = thread.Name;
            String userName = null;
            String hostName = null;

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

            return new LogEntry( id, logName, creationTime, threadId, threadName, userName, hostName, message, logLevel );
        }

        public static LogEntry Create(
            String logName,
            String message,
            LogLevel logLevel )
        {
            var creationTime = OptimizedDateTime.Now;
            return Create( logName, creationTime, message, logLevel );
        }
    }
}