using System;
using System.Collections.Generic;

namespace Foundation.Log;

public interface ILogFactory : IDisposable
{
    string? FileName { get; }
    ILog GetLog(string? name);
    void Write(IEnumerable<LogEntry> logEntries);
}