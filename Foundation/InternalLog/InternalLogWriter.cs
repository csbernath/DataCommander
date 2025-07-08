using System.Collections.Generic;
using Foundation.Core;
using Foundation.Log;

namespace Foundation.InternalLog;

public sealed class InternalLogWriter : ILogWriter
{
    private readonly List<LogEntry> _logEntries;
    private readonly ILogWriter _textLogWriter;

    public InternalLogWriter()
    {
        _logEntries = new  List<LogEntry>();
        _textLogWriter = new TextLogWriter(TraceWriter.Instance, new TextLogFormatter());
    }
    
    public List<LogEntry> LogEntries => _logEntries;

    public void Dispose()
    {
        _textLogWriter.Dispose();
    }

    public void Open()
    {
        _textLogWriter.Open();
    }

    public void Write(LogEntry logEntry)
    {
        _logEntries.Add(logEntry);
        _textLogWriter.Write(logEntry);
    }

    public void Flush()
    {
        _textLogWriter.Flush();
    }

    public void Close()
    {
        _textLogWriter.Close();
    }
}