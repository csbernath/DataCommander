using System;
using Foundation.Core;
using Foundation.Log;

namespace Foundation.InternalLog;

internal sealed class InternalLogger(ILogWriter logWriter, IDateTimeProvider dateTimeProvider, string? logName) : ILogger
{
    void IDisposable.Dispose()
    {
    }

    bool ILogger.IsEnabled(LogLevel logLevel) => throw new NotImplementedException();

    void ILogger.Write(LogLevel logLevel, string message)
    {
        var now = dateTimeProvider.Now;
        var logEntry = LogEntryFactory.Create(logName, now, message, logLevel);
        logWriter.Write(logEntry);
    }
}