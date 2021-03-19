using System;

namespace Foundation.Log
{
    public interface ILog : IDisposable
    {
        void Write(LogLevel logLevel, string message);
        bool IsEnabled(LogLevel logLevel);
    }
}