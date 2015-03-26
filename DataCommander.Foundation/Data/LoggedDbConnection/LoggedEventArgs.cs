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
        private readonly long timestamp;
        private readonly DateTime dateTime;

        /// <summary>
        /// 
        /// </summary>
        public LoggedEventArgs()
        {
            this.timestamp = Stopwatch.GetTimestamp();
            this.dateTime = LocalTime.Default.Now;
        }

        /// <summary>
        /// 
        /// </summary>
        public long Timestamp
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