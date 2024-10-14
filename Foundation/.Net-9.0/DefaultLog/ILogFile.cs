using System;
using Foundation.Log;

namespace Foundation.DefaultLog;

internal interface ILogFile : IDisposable
{
    string? FileName { get; }
    void Open();
    void Write(LogEntry entry);
    void Flush();
    void Close();
}