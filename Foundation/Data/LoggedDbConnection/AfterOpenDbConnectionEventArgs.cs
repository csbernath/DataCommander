using System;

namespace Foundation.Data.LoggedDbConnection
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class AfterOpenDbConnectionEventArgs : LoggedEventArgs
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="exception"></param>
        public AfterOpenDbConnectionEventArgs(Exception exception)
        {
            Exception = exception;
        }

        /// <summary>
        /// 
        /// </summary>
        public Exception Exception { get; }
    }
}