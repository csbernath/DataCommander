using System;

namespace Foundation.Log
{
    internal sealed class NullLog : ILog
    {
        public static readonly NullLog Instance = new NullLog();

        private NullLog()
        {
        }

        #region ILog Members

        bool ILog.IsEnabled(LogLevel logLevel) => false;

        void ILog.Write(LogLevel logLevel, string message)
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