using System;
using Foundation.Log;

namespace Foundation.DefaultLog;

internal sealed class Log : ILog
{
    private readonly LogFactory _applicationLog;
    private readonly string _name;

    public Log(LogFactory applicationLog, string name)
    {
        ArgumentNullException.ThrowIfNull(applicationLog);

        _applicationLog = applicationLog;
        _name = name;
        LoggedName = name;
    }

    public string LoggedName { get; set; }

    bool ILog.IsEnabled(LogLevel logLevel) => true;

    void ILog.Write(LogLevel logLevel, string message) => _applicationLog.Write(this, logLevel, message);

    void IDisposable.Dispose()
    {
    }
}