using System;

namespace Foundation.Log;

internal sealed class NullLogger : ILogger
{
    public static readonly NullLogger Instance = new();

    private NullLogger()
    {
    }

    bool ILogger.IsEnabled(LogLevel logLevel) => false;

    void ILogger.Write(LogLevel logLevel, string message)
    {
    }

    void IDisposable.Dispose()
    {
    }
}