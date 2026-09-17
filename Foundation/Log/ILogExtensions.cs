using System;

namespace Foundation.Log;

public static class LogExtensions
{
    public static bool IsTraceEnabled(this ILogger logger)
    {
        ArgumentNullException.ThrowIfNull(logger);
        return logger.IsEnabled(LogLevel.Trace);
    }

    public static void LogError(this ILogger logger, string message)
    {
        ArgumentNullException.ThrowIfNull(logger);
        logger.Write(LogLevel.Error, message);
    }

    public static void LogError(this ILogger logger, string format, params object[] args)
    {
        ArgumentNullException.ThrowIfNull(logger);
        var message = string.Format(format, args);
        logger.LogError(message);
    }

    public static void LogTrace(this ILogger logger, string message)
    {
        ArgumentNullException.ThrowIfNull(logger);
        logger.Write(LogLevel.Trace, message);
    }

    public static void LogTrace(this ILogger logger, string format, params object?[] args)
    {
        ArgumentNullException.ThrowIfNull(logger);
        var message = string.Format(format, args);
        logger.LogTrace(message);
    }

    public static void LogTrace(this ILogger logger, CallerInformation callerInformation, string message)
    {
        ArgumentNullException.ThrowIfNull(logger);
        ArgumentNullException.ThrowIfNull(callerInformation);

        var messageWithCallerInformation =
            $"CallerInformation: {callerInformation.CallerMemberName},{callerInformation.CallerFilePath},{callerInformation.CallerLineNumber}\r\n{message}";

        logger.LogTrace(messageWithCallerInformation);
    }

    public static void LogTrace(this ILogger logger, CallerInformation callerInformation, string format, params object[] args)
    {
        ArgumentNullException.ThrowIfNull(logger);
        ArgumentNullException.ThrowIfNull(callerInformation);
        var message = string.Format(format, args);
        logger.LogTrace(callerInformation, message);
    }

    public static void Write(this ILogger logger, LogLevel logLevel, string format, params object?[] args)
    {
        ArgumentNullException.ThrowIfNull(logger);
        var message = string.Format(format, args);
        logger.Write(logLevel, message);
    }
}