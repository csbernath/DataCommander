namespace DataCommander.Foundation.Data
{
    using System;

#if FOUNDATION_3_5

#else

#endif

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
        public AfterOpenDbConnectionEventArgs( Exception exception )
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