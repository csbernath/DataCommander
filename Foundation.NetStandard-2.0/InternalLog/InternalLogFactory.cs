using System;
using Foundation.DefaultLog;
using Foundation.Diagnostics;
using Foundation.Log;

namespace Foundation.InternalLog
{
    internal sealed class InternalLogFactory : ILogFactory
    {
        public static readonly InternalLogFactory Instance = new InternalLogFactory();

        private static TextLogWriter _textLogWriter;

        private InternalLogFactory() => _textLogWriter = new TextLogWriter(TraceWriter.Instance);

        void IDisposable.Dispose()
        {
        }

        string ILogFactory.FileName => null;

        ILog ILogFactory.GetLog(string name)
        {
            return new InternalLog(_textLogWriter, name);
        }
    }

    internal sealed class InternalLog : ILog
    {
        private readonly ILogWriter _logWriter;
        private readonly string _name;

        public InternalLog(ILogWriter logWriter, string name)
        {
            _logWriter = logWriter;
            _name = name;
        }

        bool ILog.IsErrorEnabled => throw new NotImplementedException();

        bool ILog.IsWarningEnabled => throw new NotImplementedException();

        bool ILog.IsInformationEnabled => throw new NotImplementedException();

        bool ILog.IsTraceEnabled => throw new NotImplementedException();

        bool ILog.IsDebugEnabled => throw new NotImplementedException();

        void ILog.Debug(string message)
        {
            throw new NotImplementedException();
        }

        void ILog.Debug(string format, params object[] args)
        {
            throw new NotImplementedException();
        }

        void ILog.Debug(Func<string> getMessage)
        {
            throw new NotImplementedException();
        }

        void IDisposable.Dispose()
        {
            throw new NotImplementedException();
        }

        void ILog.Error(string message)
        {
            throw new NotImplementedException();
        }

        void ILog.Error(string format, params object[] args)
        {
            throw new NotImplementedException();
        }

        void ILog.Error(Func<string> getMessage)
        {
            throw new NotImplementedException();
        }

        void ILog.Information(string message)
        {
            throw new NotImplementedException();
        }

        void ILog.Information(string format, params object[] args)
        {
            throw new NotImplementedException();
        }

        void ILog.Information(Func<string> getMessage)
        {
            throw new NotImplementedException();
        }

        void ILog.Trace(string message)
        {
            var logEntry = LogEntryFactory.Create(_name, LocalTime.Default.Now, message, LogLevel.Trace);
            _logWriter.Write(logEntry);
        }

        void ILog.Trace(string format, params object[] args)
        {
            var message = string.Format(format, args);
            var logEntry = LogEntryFactory.Create(_name, LocalTime.Default.Now, message, LogLevel.Trace);
            _logWriter.Write(logEntry);
        }

        void ILog.Trace(Func<string> getMessage)
        {
            throw new NotImplementedException();
        }

        void ILog.Warning(string message)
        {
            throw new NotImplementedException();
        }

        void ILog.Warning(string format, params object[] args)
        {
            throw new NotImplementedException();
        }

        void ILog.Warning(Func<string> getMessage)
        {
            throw new NotImplementedException();
        }

        void ILog.Write(LogLevel logLevel, string message)
        {
            throw new NotImplementedException();
        }

        void ILog.Write(LogLevel logLevel, string format, params object[] args)
        {
            var message = string.Format(format, args);
            var logEntry = LogEntryFactory.Create(_name, LocalTime.Default.Now, message, logLevel);
            _logWriter.Write(logEntry);
        }

        void ILog.Write(LogLevel logLevel, Func<string> getMessage)
        {
            throw new NotImplementedException();
        }
    }
}