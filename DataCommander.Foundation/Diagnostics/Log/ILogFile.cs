using System;

namespace Foundation.Diagnostics.Log
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