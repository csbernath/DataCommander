using System;
using System.Diagnostics;

namespace Foundation.Data.LoggedDbConnection
{
#if FOUNDATION_3_5

#else

#endif

    /// <summary>
    /// 
    /// </summary>
    public class LoggedEventArgs : EventArgs
    {
        /// <summary>
        /// 
        /// </summary>
        public LoggedEventArgs()
        {
            Timestamp = Stopwatch.GetTimestamp();
            DateTime = LocalTime.Default.Now;
        }

        /// <summary>
        /// 
        /// </summary>
        public long Timestamp { get; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime DateTime { get; }
    }
}