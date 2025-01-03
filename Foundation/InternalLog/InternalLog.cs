using System;
using Foundation.Core;
using Foundation.Log;

namespace Foundation.InternalLog;

internal sealed class InternalLog(ILogWriter logWriter, IDateTimeProvider dateTimeProvider, string? logName) : ILog
{
    void IDisposable.Dispose()
    {
    }

    bool ILog.IsEnabled(LogLevel logLevel) => throw new NotImplementedException();

    void ILog.Write(LogLevel logLevel, string message)
    {
        var now = dateTimeProvider.Now;
        var logEntry = LogEntryFactory.Create(logName, now, message, logLevel);
        logWriter.Write(logEntry);
    }
}