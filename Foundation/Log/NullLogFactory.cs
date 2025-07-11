﻿using System;
using System.Collections.Generic;

namespace Foundation.Log;

public sealed class NullLogFactory : ILogFactory
{
    public static readonly NullLogFactory Instance = new();

    private NullLogFactory()
    {
    }

    string? ILogFactory.FileName => null;

    ILog ILogFactory.GetLog(string? name) => NullLog.Instance;
    
    void ILogFactory.Write(IEnumerable<LogEntry> logEntries) => throw new NotImplementedException();

    void IDisposable.Dispose()
    {
    }
}