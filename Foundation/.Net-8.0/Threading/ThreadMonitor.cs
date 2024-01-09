using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using Foundation.Text;

namespace Foundation.Threading;

/// <summary>
/// Monitors threads in the current process.
/// <see cref="WorkerThread"/> instances are automatically added to this singleton.
/// </summary>
public static class ThreadMonitor
{
    private static readonly SortedDictionary<int, WorkerThread> Threads = new();

    private static readonly StringTableColumnInfo<WorkerThread>[] ThreadColumns =
    [
        new("ManagedThreadId", StringTableColumnAlign.Right, t => t.ManagedThreadId.ToString()),
        new("Name", StringTableColumnAlign.Left, t => t.Name),
        new("State", StringTableColumnAlign.Left, t => t.ThreadState.ToString()),
        new("StartTime", StringTableColumnAlign.Left, t => ToString(t.StartTime)),
        new("StopTime", StringTableColumnAlign.Left, t => ToString(t.StopTime)),
        new("Elapsed", StringTableColumnAlign.Left,
            t => t.ThreadState == ThreadState.Stopped ? (t.StopTime - t.StartTime).ToString() : null),
        new("Priority", StringTableColumnAlign.Left, t => GetPriority(t.Thread)),
        new("IsBackground", StringTableColumnAlign.Left, t => IsBackground(t.Thread)),
        new("IsThreadPoolThread", StringTableColumnAlign.Left, t => t.Thread.IsThreadPoolThread.ToString())
    ];

    /// <summary>
    /// 
    /// </summary>
    public static int Count => Threads.Count;

    /// <summary>
    /// Retrieves the state of the threads in table format.
    /// </summary>
    public static string ToStringTableString()
    {
        string stringTableString;
        lock (Threads)
            stringTableString = Threads.Values.ToString(ThreadColumns);

        return stringTableString;
    }

    internal static void Add(WorkerThread thread)
    {
        ArgumentNullException.ThrowIfNull(thread);

        lock (Threads)
            Threads.Add(thread.ManagedThreadId, thread);
    }

    /// <summary>
    /// Tries to join (<see cref="System.Threading.Thread.Join(int)"/>) threads and removes the joined threads from the list of monitored threads.
    /// </summary>
    public static void Join(int millisecondsTimeout)
    {
        var removableThreads = new List<WorkerThread>();

        WorkerThread[] currentThreads;
        lock (Threads)
            currentThreads = Threads.Values.ToArray();

        var remaining = TimeSpan.FromMilliseconds(millisecondsTimeout);

        foreach (var thread in currentThreads)
        {
            if (thread.ThreadState == ThreadState.Unstarted)
                removableThreads.Add(thread);
            else
            {
                var stopwatch = System.Diagnostics.Stopwatch.StartNew();
                var joined = thread.Thread.Join(remaining);
                stopwatch.Stop();

                if (remaining >= stopwatch.Elapsed)
                    remaining -= stopwatch.Elapsed;
                else
                    remaining = TimeSpan.Zero;

                if (joined)
                    removableThreads.Add(thread);
            }
        }

        if (removableThreads.Count > 0)
            lock (Threads)
                foreach (var thread in removableThreads)
                    Threads.Remove(thread.ManagedThreadId);
    }

    private static string ToString(DateTime dateTime)
    {
        var s = dateTime == DateTime.MinValue
            ? null
            : dateTime.ToString("yyyy.MM.dd HH:mm:ss.ffffff", CultureInfo.InvariantCulture);
        return s;
    }

    private static string GetPriority(Thread thread)
    {
        string priority = null;
        if (thread.IsAlive)
        {
            try
            {
                priority = thread.Priority.ToString();
            }
            catch
            {
            }
        }

        return priority;
    }

    private static string IsBackground(Thread thread)
    {
        string isBackground = null;
        if (thread.IsAlive)
        {
            try
            {
                isBackground = thread.IsBackground.ToString();
            }
            catch
            {
            }
        }

        return isBackground;
    }
}