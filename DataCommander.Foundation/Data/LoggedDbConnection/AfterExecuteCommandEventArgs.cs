namespace DataCommander.Foundation.Data.LoggedDbConnection
{
    using System;

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
            this.Command = command;
            this.Exception = exception;
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