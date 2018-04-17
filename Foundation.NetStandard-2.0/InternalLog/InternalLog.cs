using System;
using Foundation.DefaultLog;
using Foundation.Log;

namespace Foundation.InternalLog
{
    internal sealed class InternalLog : ILog
    {
        private readonly ILogWriter _logWriter;
        private readonly string _name;

        public InternalLog(ILogWriter logWriter, string name)
        {
            _logWriter = logWriter;
            _name = name;
        }

        void IDisposable.Dispose()
        {
        }

        bool ILog.IsEnabled(LogLevel logLevel)
        {
            throw new NotImplementedException();
        }

        void ILog.Write(LogLevel logLevel, string message)
        {
            var logEntry = LogEntryFactory.Create(_name, LocalTime.Default.Now, message, logLevel);
            _logWriter.Write(logEntry);
        }
    }
}