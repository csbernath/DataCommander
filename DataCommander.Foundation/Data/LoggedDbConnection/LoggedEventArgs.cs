namespace DataCommander.Foundation.Data
{
    using System;
    using System.Diagnostics;
#if FOUNDATION_3_5

#else

#endif

    /// <summary>
    /// 
    /// </summary>
    public class LoggedEventArgs : EventArgs
    {
        private readonly Int64 timestamp;
        private readonly DateTime dateTime;

        /// <summary>
        /// 
        /// </summary>
        public LoggedEventArgs()
        {
            this.timestamp = Stopwatch.GetTimestamp();
            this.dateTime = OptimizedDateTime.Now;
        }

        /// <summary>
        /// 
        /// </summary>
        public Int64 Timestamp
        {
            get
            {
                return this.timestamp;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public DateTime DateTime
        {
            get
            {
                return this.dateTime;
            }
        }
    }
}