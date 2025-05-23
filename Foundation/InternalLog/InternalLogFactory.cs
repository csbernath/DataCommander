﻿using System;
using Foundation.Core;
using Foundation.Log;

namespace Foundation.InternalLog;

public sealed class InternalLogFactory : ILogFactory
{
    public static readonly InternalLogFactory Instance = new();

    private static TextLogWriter? _textLogWriter;

    private InternalLogFactory() => _textLogWriter = new TextLogWriter(TraceWriter.Instance, new TextLogFormatter());

    void IDisposable.Dispose()
    {
    }

    string? ILogFactory.FileName => null;

    ILog ILogFactory.GetLog(string? name) => new InternalLog(_textLogWriter!, LocalTime.Default, name);
}