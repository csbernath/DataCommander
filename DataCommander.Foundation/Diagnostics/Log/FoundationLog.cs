namespace DataCommander.Foundation.Diagnostics
{
    using System;

    internal sealed class FoundationLog : ILog
    {
        private readonly FoundationLogFactory applicationLog;
        private String name;
        private String loggedName;

        public FoundationLog( FoundationLogFactory applicationLog, String name )
        {
            this.applicationLog = applicationLog;
            this.name = name;
            this.loggedName = name;
        }

        public String LoggedName
        {
            get
            {
                return this.loggedName;
            }

            set
            {
                this.loggedName = value;
            }
        }

        #region ILog Members

        bool ILog.IsErrorEnabled
        {
            get
            {
                return true;
            }
        }

        bool ILog.IsWarningEnabled
        {
            get
            {
                return true;
            }
        }

        bool ILog.IsInformationEnabled
        {
            get
            {
                return true;
            }
        }

        bool ILog.IsTraceEnabled
        {
            get
            {
                return true;
            }
        }

        bool ILog.IsDebugEnabled
        {
            get
            {
                return true;
            }
        }

        void ILog.Debug( String message )
        {
            this.applicationLog.Write( this, LogLevel.Debug, message );
        }

        void ILog.Debug( String format, params Object[] args )
        {
            this.applicationLog.Write( this, LogLevel.Debug, format, args );
        }

        void ILog.Debug( Func<String> getMessage )
        {
            this.applicationLog.Write( this, LogLevel.Debug, getMessage );
        }

        void ILog.Trace( String message )
        {
            this.applicationLog.Write( this, LogLevel.Trace, message );
        }

        void ILog.Trace( String format, params Object[] args )
        {
            this.applicationLog.Write( this, LogLevel.Trace, format, args );
        }

        void ILog.Trace( Func<String> getMessage )
        {
            this.applicationLog.Write( this, LogLevel.Trace, getMessage );
        }

        void ILog.Information( String message )
        {
            this.applicationLog.Write( this, LogLevel.Information, message );
        }

        void ILog.Information( String format, params Object[] args )
        {
            this.applicationLog.Write( this, LogLevel.Information, format, args );
        }

        void ILog.Information( Func<String> getMessage )
        {
            this.applicationLog.Write( this, LogLevel.Information, getMessage );
        }

        void ILog.Warning( String message )
        {
            this.applicationLog.Write( this, LogLevel.Warning, message );
        }

        void ILog.Warning( String format, params Object[] args )
        {
            this.applicationLog.Write( this, LogLevel.Warning, format, args );
        }

        void ILog.Warning( Func<String> getMessage )
        {
            this.applicationLog.Write( this, LogLevel.Warning, getMessage );
        }

        void ILog.Error( String message )
        {
            this.applicationLog.Write( this, LogLevel.Error, message );
        }

        void ILog.Error( String format, params Object[] args )
        {
            this.applicationLog.Write( this, LogLevel.Error, format, args );
        }

        void ILog.Error( Func<String> getMessage )
        {
            this.applicationLog.Write( this, LogLevel.Error, getMessage );
        }

        void ILog.Write( LogLevel logLevel, String message )
        {
            this.applicationLog.Write( this, logLevel, message );
        }

        void ILog.Write( LogLevel logLevel, string format, params object[] args )
        {
            this.applicationLog.Write( this, logLevel, format, args );
        }

        void ILog.Write( LogLevel logLevel, Func<String> getMessage )
        {
            this.applicationLog.Write( this, logLevel, getMessage );
        }

        #endregion

        #region IDisposable Members

        void IDisposable.Dispose()
        {
        }

        #endregion
    }
}