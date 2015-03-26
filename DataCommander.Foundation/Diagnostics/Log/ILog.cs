namespace DataCommander.Foundation.Diagnostics
{
    using System;

    /// <summary>
    /// 
    /// </summary>
    public interface ILog : IDisposable
    {
        /// <summary>
        /// 
        /// </summary>
        bool IsErrorEnabled
        {
            get;
        }

        /// <summary>
        /// 
        /// </summary>
        bool IsWarningEnabled
        {
            get;
        }

        /// <summary>
        /// 
        /// </summary>
        bool IsInformationEnabled
        {
            get;
        }

        /// <summary>
        /// 
        /// </summary>
        bool IsTraceEnabled
        {
            get;
        }

        /// <summary>
        /// 
        /// </summary>
        bool IsDebugEnabled
        {
            get;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        void Debug( string message );
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        void Debug( string format, params object[] args );
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="getMessage"></param>
        void Debug( Func<string> getMessage );

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        void Trace( string message );
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        void Trace( string format, params object[] args );
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="getMessage"></param>
        void Trace( Func<string> getMessage );

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        void Information( string message );

        /// <summary>
        /// 
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        void Information( string format, params object[] args );

        /// <summary>
        /// 
        /// </summary>
        /// <param name="getMessage"></param>
        void Information( Func<string> getMessage );

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        void Warning( string message );

        /// <summary>
        /// 
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        void Warning( string format, params object[] args );

        /// <summary>
        /// 
        /// </summary>
        /// <param name="getMessage"></param>
        void Warning( Func<string> getMessage );
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        void Error( string message );
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        void Error( string format, params object[] args );
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="getMessage"></param>
        void Error( Func<string> getMessage );

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logLevel"></param>
        /// <param name="message"></param>
        void Write( LogLevel logLevel, string message );
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="logLevel"></param>
        /// <param name="format"></param>
        /// <param name="args"></param>
        void Write( LogLevel logLevel, string format, params object[] args );
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="logLevel"></param>
        /// <param name="getMessage"></param>
        void Write( LogLevel logLevel, Func<string> getMessage );
    }
}