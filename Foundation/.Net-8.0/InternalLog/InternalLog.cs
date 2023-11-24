using System;
using Foundation.Core;
using Foundation.Log;

namespace Foundation.InternalLog;

internal sealed class InternalLog : ILog
{
    private readonly ILogWriter _logWriter;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly string _name;

    public InternalLog(ILogWriter logWriter, IDateTimeProvider dateTimeProvider, string name)
    {
        _logWriter = logWriter;
        _name = name;
        _dateTimeProvider = dateTimeProvider;
    }

    void IDisposable.Dispose()
    {
    }

    bool ILog.IsEnabled(LogLevel logLevel)
    {
        throw new NotImplementedException();
    }

    void ILog.Write(LogLevel logLevel, string message)
    {
        var now = _dateTimeProvider.Now;
        var logEntry = LogEntryFactory.Create(_name, now, message, logLevel);
        _logWriter.Write(logEntry);
    }
}