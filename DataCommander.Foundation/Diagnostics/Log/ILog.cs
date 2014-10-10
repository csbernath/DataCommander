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
        Boolean IsErrorEnabled
        {
            get;
        }

        /// <summary>
        /// 
        /// </summary>
        Boolean IsWarningEnabled
        {
            get;
        }

        /// <summary>
        /// 
        /// </summary>
        Boolean IsInformationEnabled
        {
            get;
        }

        /// <summary>
        /// 
        /// </summary>
        Boolean IsTraceEnabled
        {
            get;
        }

        /// <summary>
        /// 
        /// </summary>
        Boolean IsDebugEnabled
        {
            get;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        void Debug( String message );
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        void Debug( String format, params Object[] args );
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="getMessage"></param>
        void Debug( Func<String> getMessage );

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        void Trace( String message );
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        void Trace( String format, params Object[] args );
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="getMessage"></param>
        void Trace( Func<String> getMessage );

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        void Information( String message );

        /// <summary>
        /// 
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        void Information( String format, params Object[] args );

        /// <summary>
        /// 
        /// </summary>
        /// <param name="getMessage"></param>
        void Information( Func<String> getMessage );

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        void Warning( String message );

        /// <summary>
        /// 
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        void Warning( String format, params Object[] args );

        /// <summary>
        /// 
        /// </summary>
        /// <param name="getMessage"></param>
        void Warning( Func<String> getMessage );
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        void Error( String message );
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        void Error( String format, params Object[] args );
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="getMessage"></param>
        void Error( Func<String> getMessage );

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logLevel"></param>
        /// <param name="message"></param>
        void Write( LogLevel logLevel, String message );
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="logLevel"></param>
        /// <param name="format"></param>
        /// <param name="args"></param>
        void Write( LogLevel logLevel, String format, params Object[] args );
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="logLevel"></param>
        /// <param name="getMessage"></param>
        void Write( LogLevel logLevel, Func<String> getMessage );
    }
}