using System;
using System.Diagnostics;
using Foundation.Core;

namespace Foundation.Data.LoggedDbConnection;

public class LoggedEventArgs : EventArgs
{
    public long Timestamp { get; } = Stopwatch.GetTimestamp();
    public DateTime DateTime { get; } = LocalTime.Default.Now;
}