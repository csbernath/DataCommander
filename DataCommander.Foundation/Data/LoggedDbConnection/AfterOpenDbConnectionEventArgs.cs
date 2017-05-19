namespace DataCommander.Foundation.Data.LoggedDbConnection
{
    using System;

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
            this.Exception = exception;
        }

        /// <summary>
        /// 
        /// </summary>
        public Exception Exception { get; }
    }
}