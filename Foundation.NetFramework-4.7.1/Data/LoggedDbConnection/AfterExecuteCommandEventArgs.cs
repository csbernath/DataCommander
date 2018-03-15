using System;

namespace Foundation.Data.LoggedDbConnection
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class AfterExecuteCommandEventArgs : LoggedEventArgs
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <param name="exception"></param>
        public AfterExecuteCommandEventArgs(
            LoggedDbCommandInfo command,
            Exception exception)
        {
            Command = command;
            Exception = exception;
        }

        /// <summary>
        /// 
        /// </summary>
        public LoggedDbCommandInfo Command { get; }

        /// <summary>
        /// 
        /// </summary>
        public Exception Exception { get; }
    }

}