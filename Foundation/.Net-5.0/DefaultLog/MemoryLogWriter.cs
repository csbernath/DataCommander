using System;
using System.Collections.Generic;
using Foundation.Log;

namespace Foundation.DefaultLog;

internal sealed class MemoryLogWriter : ILogWriter
{
    private readonly ICollection<LogEntry> _logEntries;

    public MemoryLogWriter()
    {
        _logEntries = new List<LogEntry>();
    }

    public IEnumerable<LogEntry> LogEntries => _logEntries;

    #region ILogWriter Members

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

    #endregion

    #region IDisposable Members

    void IDisposable.Dispose()
    {
    }

    #endregion
}