using System;
using Foundation.Assertions;
using Foundation.Log;

namespace Foundation.DefaultLog
{
    internal sealed class Log : ILog
    {
        #region Private Fields

        private readonly LogFactory _applicationLog;
        private string _name;

        #endregion

        public Log(LogFactory applicationLog, string name)
        {
            Assert.IsNotNull(applicationLog);

            _applicationLog = applicationLog;
            _name = name;
            LoggedName = name;
        }

        public string LoggedName { get; set; }

        #region ILog Members

        bool ILog.IsEnabled(LogLevel logLevel)
        {
            throw new NotImplementedException();
        }

        void ILog.Write(LogLevel logLevel, string message) => _applicationLog.Write(this, logLevel, message);

        #endregion

        #region IDisposable Members

        void IDisposable.Dispose()
        {
        }

        #endregion
    }
}