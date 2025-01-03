using System;

namespace Foundation.Log;

internal sealed class NullLog : ILog
{
    public static readonly NullLog Instance = new();

    private NullLog()
    {
    }

    bool ILog.IsEnabled(LogLevel logLevel) => false;

    void ILog.Write(LogLevel logLevel, string message)
    {
    }

    void IDisposable.Dispose()
    {
    }
}