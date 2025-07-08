using System;
using System.Collections.Generic;
using Foundation.Core;
using Foundation.Log;

namespace Foundation.InternalLog;

public sealed class InternalLogFactory : ILogFactory
{
    public static readonly InternalLogFactory Instance = new();
    public static readonly InternalLogWriter InternalLogWriter = new();

    void IDisposable.Dispose()
    {
    }

    string? ILogFactory.FileName => null;

    ILog ILogFactory.GetLog(string? name) => new InternalLog(InternalLogWriter, LocalTime.Default, name);
    
    void ILogFactory.Write(IEnumerable<LogEntry> logEntries)
    {
        throw new NotImplementedException();
    }
}