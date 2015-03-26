namespace DataCommander.Foundation.Diagnostics
{
    using System;

#if FOUNDATION_3_5
#else

#endif

    internal sealed class NullLog : ILog
    {
        private static readonly NullLog instance = new NullLog();

        private NullLog()
        {
        }

        public static NullLog Instance
        {
            get
            {
                return instance;
            }
        }

        #region ILog Members

        bool ILog.IsErrorEnabled
        {
            get
            {
                return false;
            }
        }

        bool ILog.IsWarningEnabled
        {
            get
            {
                return false;
            }
        }

        bool ILog.IsInformationEnabled
        {
            get
            {
                return false;
            }
        }

        bool ILog.IsTraceEnabled
        {
            get
            {
                return false;
            }
        }

        bool ILog.IsDebugEnabled
        {
            get
            {
                return false;
            }
        }

        void ILog.Debug( string format )
        {
        }

        void ILog.Debug( string format, params object[] args )
        {
        }

        void ILog.Debug( Func<string> getMessage )
        {
        }

        void ILog.Trace( string message )
        {
        }

        void ILog.Trace( string format, params object[] args )
        {
        }

        void ILog.Trace( Func<string> getMessage )
        {
        }

        void ILog.Information( string message )
        {
        }

        void ILog.Information( string format, params object[] args )
        {
        }

        void ILog.Information( Func<string> getMessage )
        {
        }

        void ILog.Warning( string message )
        {
        }

        void ILog.Warning( string format, params object[] args )
        {
        }

        void ILog.Warning( Func<string> getMessage )
        {
        }

        void ILog.Error( string message )
        {
        }

        void ILog.Error( string format, params object[] args )
        {
        }

        void ILog.Error( Func<string> getMessage )
        {
        }

        void ILog.Write( LogLevel logLevel, string message )
        {
        }

        void ILog.Write( LogLevel logLevel, string format, params object[] args )
        {
        }

        void ILog.Write( LogLevel logLevel, Func<string> getMessage )
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