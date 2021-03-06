﻿using System;

namespace Foundation.Log
{
    internal sealed class NullLog : ILog
    {
        public static readonly NullLog Instance = new NullLog();

        private NullLog()
        {
        }

        #region ILog Members

        bool ILog.IsErrorEnabled => false;

        bool ILog.IsWarningEnabled => false;

        bool ILog.IsInformationEnabled => false;

        bool ILog.IsTraceEnabled => false;

        bool ILog.IsDebugEnabled => false;

        void ILog.Debug(string format)
        {
        }

        void ILog.Debug(string format, params object[] args)
        {
        }

        void ILog.Debug(Func<string> getMessage)
        {
        }

        void ILog.Trace(string message)
        {
        }

        void ILog.Trace(string format, params object[] args)
        {
        }

        void ILog.Trace(Func<string> getMessage)
        {
        }

        void ILog.Information(string message)
        {
        }

        void ILog.Information(string format, params object[] args)
        {
        }

        void ILog.Information(Func<string> getMessage)
        {
        }

        void ILog.Warning(string message)
        {
        }

        void ILog.Warning(string format, params object[] args)
        {
        }

        void ILog.Warning(Func<string> getMessage)
        {
        }

        void ILog.Error(string message)
        {
        }

        void ILog.Error(string format, params object[] args)
        {
        }

        void ILog.Error(Func<string> getMessage)
        {
        }

        void ILog.Write(LogLevel logLevel, string message)
        {
        }

        void ILog.Write(LogLevel logLevel, string format, params object[] args)
        {
        }

        void ILog.Write(LogLevel logLevel, Func<string> getMessage)
        {
        }

        #endregion

        #region IDisposable Members

        void IDisposable.Dispose()
        {
        }

        #endregion
    }
}