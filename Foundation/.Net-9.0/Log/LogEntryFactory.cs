using System;
using System.Threading;

namespace Foundation.Log;

public static class LogEntryFactory
{
    private static long _id;

    public static LogEntry Create(string logName, DateTime creationTime, string message, LogLevel logLevel)
    {
        long id = Interlocked.Increment(ref _id);
        Thread thread = Thread.CurrentThread;
        int threadId = thread.ManagedThreadId;
        string threadName = thread.Name;
        string userName = null;
        string hostName = null;

        return new LogEntry(id, logName, creationTime, threadId, threadName, userName, hostName, message, logLevel);
    }
}