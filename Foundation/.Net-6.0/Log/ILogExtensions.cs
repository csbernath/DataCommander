using System;
using Foundation.Assertions;

namespace Foundation.Log;

public static class LogExtensions
{
    public static bool IsTraceEnabled(this ILog log)
    {
        ArgumentNullException.ThrowIfNull(log, nameof(log));
        return log.IsEnabled(LogLevel.Trace);
    }

    public static void Error(this ILog log, string message)
    {
        ArgumentNullException.ThrowIfNull(log, nameof(log));
        log.Write(LogLevel.Error, message);
    }

    public static void Error(this ILog log, string format, params object[] args)
    {
        ArgumentNullException.ThrowIfNull(log, nameof(log));
        var message = string.Format(format, args);
        log.Error(message);
    }

    public static void Trace(this ILog log, string message)
    {
        ArgumentNullException.ThrowIfNull(log, nameof(log));
        log.Write(LogLevel.Trace, message);
    }

    public static void Trace(this ILog log, string format, params object[] args)
    {
        ArgumentNullException.ThrowIfNull(log, nameof(log));
        var message = string.Format(format, args);
        log.Trace(message);
    }

    public static void Trace(this ILog log, CallerInformation callerInformation, string message)
    {
        ArgumentNullException.ThrowIfNull(log, nameof(log));
        ArgumentNullException.ThrowIfNull(callerInformation, nameof(callerInformation));

        var messageWithCallerInformation =
            $"CallerInformation: {callerInformation.CallerMemberName},{callerInformation.CallerFilePath},{callerInformation.CallerLineNumber}\r\n{message}";

        log.Trace(messageWithCallerInformation);
    }

    public static void Trace(this ILog log, CallerInformation callerInformation, string format, params object[] args)
    {
        ArgumentNullException.ThrowIfNull(log, nameof(log));
        ArgumentNullException.ThrowIfNull(callerInformation, nameof(callerInformation));
        var message = string.Format(format, args);
        log.Trace(callerInformation, message);
    }

    public static void Write(this ILog log, LogLevel logLevel, string format, params object[] args)
    {
        ArgumentNullException.ThrowIfNull(log, nameof(log));
        var message = string.Format(format, args);
        log.Write(logLevel, message);
    }
}