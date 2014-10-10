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

        void ILog.Debug( String format )
        {
        }

        void ILog.Debug( String format, params Object[] args )
        {
        }

        void ILog.Debug( Func<String> getMessage )
        {
        }

        void ILog.Trace( String message )
        {
        }

        void ILog.Trace( String format, params Object[] args )
        {
        }

        void ILog.Trace( Func<String> getMessage )
        {
        }

        void ILog.Information( String message )
        {
        }

        void ILog.Information( String format, params Object[] args )
        {
        }

        void ILog.Information( Func<String> getMessage )
        {
        }

        void ILog.Warning( String message )
        {
        }

        void ILog.Warning( String format, params Object[] args )
        {
        }

        void ILog.Warning( Func<String> getMessage )
        {
        }

        void ILog.Error( String message )
        {
        }

        void ILog.Error( String format, params Object[] args )
        {
        }

        void ILog.Error( Func<String> getMessage )
        {
        }

        void ILog.Write( LogLevel logLevel, String message )
        {
        }

        void ILog.Write( LogLevel logLevel, String format, params Object[] args )
        {
        }

        void ILog.Write( LogLevel logLevel, Func<String> getMessage )
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