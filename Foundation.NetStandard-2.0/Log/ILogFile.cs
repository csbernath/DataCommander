using System;

namespace Foundation.Log
{
    internal interface ILogFile : IDisposable
    {
        string FileName { get; }
        void Open();
        void Write(LogEntry entry);
        void Flush();
        void Close();
    }
}