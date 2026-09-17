using System;
using System.Collections.Generic;

namespace Foundation.Log;

public interface ILogFactory : IDisposable
{
    string? FileName { get; }
    ILogger GetLog(string? name);
    void Write(IReadOnlyCollection<LogEntry> logEntries);
}