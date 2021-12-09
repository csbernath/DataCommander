using System;
using Foundation.Log;

namespace Foundation.DefaultLog;

public sealed class ConsoleLogWriter : ILogWriter
{
    private static readonly object LockObject = new();

    private ConsoleLogWriter()
    {
    }

    static ConsoleLogWriter()
    {
        Instance = new ConsoleLogWriter();
    }

    public static ConsoleLogWriter Instance { get; }

    #region ILogWriter Members

    void ILogWriter.Open()
    {
    }

    void ILogWriter.Write(LogEntry logEntry)
    {
        lock (LockObject)
        {
            var color = Console.ForegroundColor;

            switch (logEntry.LogLevel)
            {
                case LogLevel.Error:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;

                case LogLevel.Warning:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
            }

            Console.WriteLine(logEntry.Message);

            switch (logEntry.LogLevel)
            {
                case LogLevel.Error:
                case LogLevel.Warning:
                    Console.ForegroundColor = color;
                    break;
            }
        }
    }

    void ILogWriter.Flush()
    {
    }

    void ILogWriter.Close()
    {
    }

    #endregion

    #region IDisposable Members

    void IDisposable.Dispose()
    {
    }

    #endregion
}