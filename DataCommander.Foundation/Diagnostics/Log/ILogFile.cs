namespace DataCommander.Foundation.Diagnostics
{
    using System;

    internal interface ILogFile : IDisposable
    {
        void Open();

        void Write(LogEntry entry);

        void Flush();

        void Close();
    }
}