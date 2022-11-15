using System;
using System.Threading;

namespace Foundation.Diagnostics;

internal static class MonitorExtensions
{
    public static void TryLock(object obj, Action action)
    {
        var entered = Monitor.TryEnter(obj);
        if (entered)
        {
            try
            {
                action();
            }
            finally
            {
                Monitor.Exit(obj);
            }
        }
    }
}