using System.Threading;
using Foundation.Text;

namespace Foundation.Threading;

public static class ThreadPoolMonitor
{
    private static readonly StringTableColumnInfo<ThreadPoolRow>[] ThreadPoolColumns =
    {
        new("Name", StringTableColumnAlign.Left, t => t.Name),
        new("Min", StringTableColumnAlign.Right, t => t.Min.ToString()),
        new("Active", StringTableColumnAlign.Right, t => t.Active.ToString()),
        new("Available", StringTableColumnAlign.Right, t => t.Available.ToString()),
        new("Max", StringTableColumnAlign.Right, t => t.Max.ToString())
    };

    public static string ThreadPoolToStringTableString()
    {
        ThreadPool.GetMinThreads(out var minWorkerThreads, out var minCompletionPortThreads);
        ThreadPool.GetMaxThreads(out var maxWorkerThreads, out var maxCompletionPortThreads);
        ThreadPool.GetAvailableThreads(out var availableWorkerThreads, out var availableCompletionPortThreads);

        var threadPoolRows = new[]
        {
            new ThreadPoolRow("WorkerThreads", minWorkerThreads, maxWorkerThreads - availableWorkerThreads, availableWorkerThreads, maxWorkerThreads),
            new ThreadPoolRow("CompletionPortThreads", minCompletionPortThreads, maxCompletionPortThreads - availableCompletionPortThreads,
                availableCompletionPortThreads,
                maxCompletionPortThreads)
        };

        return threadPoolRows.ToString(ThreadPoolColumns);
    }

    private sealed class ThreadPoolRow
    {
        public readonly string Name;
        public readonly int Min;
        public readonly int Active;
        public readonly int Available;
        public readonly int Max;

        public ThreadPoolRow(string name, int min, int active, int available, int max)
        {
            Name = name;
            Min = min;
            Active = active;
            Available = available;
            Max = max;
        }
    }
}