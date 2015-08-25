namespace DataCommander.Foundation.Data
{
    using System;

    /// <summary>
    /// 
    /// </summary>
    public sealed class AfterOpenDbConnectionEventArgs : LoggedEventArgs
    {
        private readonly Exception exception;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="exception"></param>
        public AfterOpenDbConnectionEventArgs(Exception exception)
        {
            this.exception = exception;
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