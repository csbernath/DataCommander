using System;

namespace Foundation.Log;

public sealed class LogEntry(
    long id,
    string? logName,
    DateTime creationTime,
    int managedThreadId,
    string? threadName,
    string? userName,
    string? hostName,
    string message,
    LogLevel logLevel)
{
    public readonly long Id = id;
    public readonly string? LogName = logName;
    public readonly DateTime CreationTime = creationTime;
    public readonly int ManagedThreadId = managedThreadId;
    public readonly string? ThreadName = threadName;
    public readonly string? HostName = hostName;
    public readonly string? UserName = userName;
    public readonly string Message = message;
    public readonly LogLevel LogLevel = logLevel;
}