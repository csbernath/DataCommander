namespace DataCommander.Foundation.Data
{
    using System;

    /// <summary>
    /// 
    /// </summary>
    public sealed class AfterExecuteCommandEventArgs : LoggedEventArgs
    {
        private readonly LoggedDbCommandInfo command;
        private readonly Exception exception;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <param name="exception"></param>
        public AfterExecuteCommandEventArgs(
            LoggedDbCommandInfo command,
            Exception exception)
        {
            this.command = command;
            this.exception = exception;
        }

        /// <summary>
        /// 
        /// </summary>
        public LoggedDbCommandInfo Command
        {
            get
            {
                return this.command;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public Exception Exception
        {
            get
            {
                return this.exception;
            }
        }
    }

}