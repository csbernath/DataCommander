using System;
using System.Collections.Generic;
using Foundation.Log;

namespace Foundation.DefaultLog;

internal sealed class MemoryLogWriter : ILogWriter
{
    private readonly ICollection<LogEntry> _logEntries = new List<LogEntry>();

    public IEnumerable<LogEntry> LogEntries => _logEntries;

    void ILogWriter.Open()
    {
    }

    void ILogWriter.Write(LogEntry logEntry)
    {
        lock (_logEntries)
        {
            _logEntries.Add(logEntry);
        }
    }

    void ILogWriter.Flush()
    {
    }

    void ILogWriter.Close()
    {
    }

    void IDisposable.Dispose()
    {
    }
}