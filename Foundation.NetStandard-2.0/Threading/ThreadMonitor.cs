using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using Foundation.Diagnostics.Assertions;
using Foundation.Linq;
using Foundation.Text;

namespace Foundation.Threading
{
    /// <summary>
    /// Monitors threads in the current process.
    /// <see cref="WorkerThread"/> instances are automatically added to this singleton.
    /// </summary>
    public static class ThreadMonitor
    {
        private static readonly SortedDictionary<int, WorkerThread> Threads = new SortedDictionary<int, WorkerThread>();

        private static readonly StringTableColumnInfo<WorkerThread>[] ThreadColumns =
        {
            new StringTableColumnInfo<WorkerThread>("ManagedThreadId", StringTableColumnAlign.Right, t => t.ManagedThreadId.ToString()),
            new StringTableColumnInfo<WorkerThread>("Name", StringTableColumnAlign.Left, t => t.Name),
            new StringTableColumnInfo<WorkerThread>("State", StringTableColumnAlign.Left, t => t.ThreadState.ToString()),
            new StringTableColumnInfo<WorkerThread>("StartTime", StringTableColumnAlign.Left, t => ToString(t.StartTime)),
            new StringTableColumnInfo<WorkerThread>("StopTime", StringTableColumnAlign.Left, t => ToString(t.StopTime)),
            new StringTableColumnInfo<WorkerThread>("Elapsed", StringTableColumnAlign.Left, t => t.ThreadState == ThreadState.Stopped ? (t.StopTime - t.StartTime).ToString() : null),
            new StringTableColumnInfo<WorkerThread>("Priority", StringTableColumnAlign.Left, t => GetPriority(t.Thread)),
            new StringTableColumnInfo<WorkerThread>("IsBackground", StringTableColumnAlign.Left, t => IsBackground(t.Thread)),
            new StringTableColumnInfo<WorkerThread>("IsThreadPoolThread", StringTableColumnAlign.Left, t => t.Thread.IsThreadPoolThread.ToString())
        };

        private static readonly StringTableColumnInfo<ThreadPoolRow>[] ThreadPoolColumns =
        {
            new StringTableColumnInfo<ThreadPoolRow>("Name", StringTableColumnAlign.Left, t => t.Name),
            new StringTableColumnInfo<ThreadPoolRow>("Min", StringTableColumnAlign.Right, t => t.Min.ToString()),
            new StringTableColumnInfo<ThreadPoolRow>("Active", StringTableColumnAlign.Right, t => t.Active.ToString()),
            new StringTableColumnInfo<ThreadPoolRow>("Available", StringTableColumnAlign.Right, t => t.Available.ToString()),
            new StringTableColumnInfo<ThreadPoolRow>("Max", StringTableColumnAlign.Right, t => t.Max.ToString())
        };

        /// <summary>
        /// 
        /// </summary>
        public static int Count => Threads.Count;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static string ThreadPoolToStringTableString()
        {
            int minWorkerThreads;
            int minCompletionPortThreads;
            int maxWorkerThreads;
            int maxCompletionPortThreads;
            int availableWorkerThreads;
            int availableCompletionPortThreads;
            ThreadPool.GetMinThreads(out minWorkerThreads, out minCompletionPortThreads);
            ThreadPool.GetMaxThreads(out maxWorkerThreads, out maxCompletionPortThreads);
            ThreadPool.GetAvailableThreads(out availableWorkerThreads, out availableCompletionPortThreads);

            var threadPoolRows = new ThreadPoolRow[2];

            threadPoolRows[0] = new ThreadPoolRow
            {
                Name = "WorkerThreads",
                Min = minWorkerThreads,
                Active = maxWorkerThreads - availableWorkerThreads,
                Available = availableWorkerThreads,
                Max = maxWorkerThreads
            };

            threadPoolRows[1] = new ThreadPoolRow
            {
                Name = "CompletionPortThreads",
                Min = minCompletionPortThreads,
                Active = maxCompletionPortThreads - availableCompletionPortThreads,
                Available = availableCompletionPortThreads,
                Max = maxCompletionPortThreads
            };

            return threadPoolRows.ToString(ThreadPoolColumns);
        }

        /// <summary>
        /// Retrieves the state of the threads in table format.
        /// </summary>
        public static string ToStringTableString()
        {
            string stringTableString;
            lock (Threads)
            {
                stringTableString = Threads.Values.ToString(ThreadColumns);
            }

            return stringTableString;
        }

        internal static void Add(WorkerThread thread)
        {
            Assert.IsNotNull(thread);

            lock (Threads)
                Threads.Add(thread.ManagedThreadId, thread);
        }

        /// <summary>
        /// Tries to join (<see cref="System.Threading.Thread.Join(int)"/>) threads and removes the joined threads from the list of monitored threads.
        /// </summary>
        public static void Join(int millisecondsTimout)
        {
            var removableThreads = new List<WorkerThread>();

            WorkerThread[] currentThreads;
            lock (Threads)
            {
                currentThreads = Threads.Values.ToArray();
            }

            var remaining = TimeSpan.FromMilliseconds(millisecondsTimout);

            foreach (var thread in currentThreads)
            {
                if (thread.ThreadState == ThreadState.Unstarted)
                {
                    removableThreads.Add(thread);
                }
                else
                {
                    var stopwatch = System.Diagnostics.Stopwatch.StartNew();
                    var joined = thread.Thread.Join(remaining);
                    stopwatch.Stop();

                    if (remaining >= stopwatch.Elapsed)
                    {
                        remaining -= stopwatch.Elapsed;
                    }
                    else
                    {
                        remaining = TimeSpan.Zero;
                    }

                    if (joined)
                    {
                        removableThreads.Add(thread);
                    }
                }
            }

            if (removableThreads.Count > 0)
            {
                lock (Threads)
                {
                    foreach (var thread in removableThreads)
                    {
                        Threads.Remove(thread.ManagedThreadId);
                    }
                }
            }
        }

        private static string ToString(DateTime dateTime)
        {
            string s;

            if (dateTime == DateTime.MinValue)
            {
                s = null;
            }
            else
            {
                s = dateTime.ToString("yyyy.MM.dd HH:mm:ss.ffffff", CultureInfo.InvariantCulture);
            }

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

        private sealed class ThreadPoolRow
        {
            public string Name;
            public int Min;
            public int Active;
            public int Available;
            public int Max;
        }
    }
}