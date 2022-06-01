using System;
using Foundation.Log;

namespace Foundation.DefaultLog;

internal sealed class Log : ILog
{
    #region Private Fields

    private readonly LogFactory _applicationLog;
    private string _name;

    #endregion

    public Log(LogFactory applicationLog, string name)
    {
        ArgumentNullException.ThrowIfNull(applicationLog);

        _applicationLog = applicationLog;
        _name = name;
        LoggedName = name;
    }

    public string LoggedName { get; set; }

    #region ILog Members

    bool ILog.IsEnabled(LogLevel logLevel) => true;

    void ILog.Write(LogLevel logLevel, string message) => _applicationLog.Write(this, logLevel, message);

    #endregion

    #region IDisposable Members

    void IDisposable.Dispose()
    {
    }

    #endregion
}