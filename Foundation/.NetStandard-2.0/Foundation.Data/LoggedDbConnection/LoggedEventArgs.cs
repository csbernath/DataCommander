using System;
using System.Diagnostics;

namespace Foundation.Data.LoggedDbConnection
{
    public class LoggedEventArgs : EventArgs
    {
        public LoggedEventArgs()
        {
            Timestamp = Stopwatch.GetTimestamp();
            DateTime = LocalTime.Default.Now;
        }

        public long Timestamp { get; }
        public DateTime DateTime { get; }
    }
}