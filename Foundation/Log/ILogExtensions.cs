using System;

namespace Foundation.Log;

public static class LogExtensions
{
    public static bool IsTraceEnabled(this ILog log)
    {
        ArgumentNullException.ThrowIfNull(log);
        return log.IsEnabled(LogLevel.Trace);
    }

    public static void LogError(this ILog log, string message)
    {
        ArgumentNullException.ThrowIfNull(log);
        log.Write(LogLevel.Error, message);
    }

    public static void LogError(this ILog log, string format, params object[] args)
    {
        ArgumentNullException.ThrowIfNull(log);
        var message = string.Format(format, args);
        log.LogError(message);
    }

    public static void LogTrace(this ILog log, string message)
    {
        ArgumentNullException.ThrowIfNull(log);
        log.Write(LogLevel.Trace, message);
    }

    public static void LogTrace(this ILog log, string format, params object?[] args)
    {
        ArgumentNullException.ThrowIfNull(log);
        var message = string.Format(format, args);
        log.LogTrace(message);
    }

    public static void LogTrace(this ILog log, CallerInformation callerInformation, string message)
    {
        ArgumentNullException.ThrowIfNull(log);
        ArgumentNullException.ThrowIfNull(callerInformation);

        var messageWithCallerInformation =
            $"CallerInformation: {callerInformation.CallerMemberName},{callerInformation.CallerFilePath},{callerInformation.CallerLineNumber}\r\n{message}";

        log.LogTrace(messageWithCallerInformation);
    }

    public static void LogTrace(this ILog log, CallerInformation callerInformation, string format, params object[] args)
    {
        ArgumentNullException.ThrowIfNull(log);
        ArgumentNullException.ThrowIfNull(callerInformation);
        var message = string.Format(format, args);
        log.LogTrace(callerInformation, message);
    }

    public static void Write(this ILog log, LogLevel logLevel, string format, params object?[] args)
    {
        ArgumentNullException.ThrowIfNull(log);
        var message = string.Format(format, args);
        log.Write(logLevel, message);
    }
}