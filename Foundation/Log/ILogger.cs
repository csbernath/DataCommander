using System;

namespace Foundation.Log;

public interface ILogger : IDisposable
{
    void Write(LogLevel logLevel, string message);
    bool IsEnabled(LogLevel logLevel);
}