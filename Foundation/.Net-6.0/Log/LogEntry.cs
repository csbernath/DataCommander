using System;

namespace Foundation.Log;

public sealed class LogEntry
{
    public readonly long Id;
    public readonly string LogName;
    public readonly DateTime CreationTime;
    public readonly int ManagedThreadId;
    public readonly string ThreadName;
    public readonly string HostName;
    public readonly string UserName;
    public readonly string Message;
    public readonly LogLevel LogLevel;

    public LogEntry(long id, string logName, DateTime creationTime, int managedThreadId, string threadName, string userName, string hostName,
        string message, LogLevel logLevel)
    {
        Id = id;
        LogName = logName;
        CreationTime = creationTime;
        ManagedThreadId = managedThreadId;
        ThreadName = threadName;
        UserName = userName;
        HostName = hostName;
        Message = message;
        LogLevel = logLevel;
    }
}