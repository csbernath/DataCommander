using System;
using Foundation.Log;

namespace Foundation.DefaultLog;

internal sealed class Logger : ILogger
{
    private readonly LogFactory _applicationLog;
    private readonly string _name;

    public Logger(LogFactory applicationLog, string name)
    {
        ArgumentNullException.ThrowIfNull(applicationLog);

        _applicationLog = applicationLog;
        _name = name;
        LoggedName = name;
    }

    public string LoggedName { get; set; }

    bool ILogger.IsEnabled(LogLevel logLevel) => true;

    void ILogger.Write(LogLevel logLevel, string message) => _applicationLog.Write(this, logLevel, message);

    void IDisposable.Dispose()
    {
    }
}