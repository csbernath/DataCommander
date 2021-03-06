﻿using System;
using Foundation.Assertions;

namespace Foundation.Log
{
    internal sealed class FoundationLog : ILog
    {
        #region Private Fields

        private readonly FoundationLogFactory _applicationLog;
        private string _name;

        #endregion

        public FoundationLog(FoundationLogFactory applicationLog, string name)
        {
            Assert.IsNotNull(applicationLog);

            _applicationLog = applicationLog;
            _name = name;
            LoggedName = name;
        }

        public string LoggedName { get; set; }

        #region ILog Members

        bool ILog.IsErrorEnabled => true;

        bool ILog.IsWarningEnabled => true;

        bool ILog.IsInformationEnabled => true;

        bool ILog.IsTraceEnabled => true;

        bool ILog.IsDebugEnabled => true;

        void ILog.Debug(string message)
        {
            _applicationLog.Write(this, LogLevel.Debug, message);
        }

        void ILog.Debug(string format, params object[] args)
        {
            _applicationLog.Write(this, LogLevel.Debug, format, args);
        }

        void ILog.Debug(Func<string> getMessage)
        {
            _applicationLog.Write(this, LogLevel.Debug, getMessage);
        }

        void ILog.Trace(string message)
        {
            _applicationLog.Write(this, LogLevel.Trace, message);
        }

        void ILog.Trace(string format, params object[] args)
        {
            _applicationLog.Write(this, LogLevel.Trace, format, args);
        }

        void ILog.Trace(Func<string> getMessage)
        {
            _applicationLog.Write(this, LogLevel.Trace, getMessage);
        }

        void ILog.Information(string message)
        {
            _applicationLog.Write(this, LogLevel.Information, message);
        }

        void ILog.Information(string format, params object[] args)
        {
            _applicationLog.Write(this, LogLevel.Information, format, args);
        }

        void ILog.Information(Func<string> getMessage)
        {
            _applicationLog.Write(this, LogLevel.Information, getMessage);
        }

        void ILog.Warning(string message)
        {
            _applicationLog.Write(this, LogLevel.Warning, message);
        }

        void ILog.Warning(string format, params object[] args)
        {
            _applicationLog.Write(this, LogLevel.Warning, format, args);
        }

        void ILog.Warning(Func<string> getMessage)
        {
            _applicationLog.Write(this, LogLevel.Warning, getMessage);
        }

        void ILog.Error(string message)
        {
            _applicationLog.Write(this, LogLevel.Error, message);
        }

        void ILog.Error(string format, params object[] args)
        {
            _applicationLog.Write(this, LogLevel.Error, format, args);
        }

        void ILog.Error(Func<string> getMessage)
        {
            _applicationLog.Write(this, LogLevel.Error, getMessage);
        }

        void ILog.Write(LogLevel logLevel, string message)
        {
            _applicationLog.Write(this, logLevel, message);
        }

        void ILog.Write(LogLevel logLevel, string format, params object[] args)
        {
            _applicationLog.Write(this, logLevel, format, args);
        }

        void ILog.Write(LogLevel logLevel, Func<string> getMessage)
        {
            _applicationLog.Write(this, logLevel, getMessage);
        }

        #endregion

        #region IDisposable Members

        void IDisposable.Dispose()
        {
        }

        #endregion
    }
}