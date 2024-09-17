using System.Threading;
using Foundation.Text;

namespace Foundation.Threading;

public static class ThreadPoolMonitor
{
    private static readonly StringTableColumnInfo<ThreadPoolRow>[] ThreadPoolColumns =
    [
        new("Name", StringTableColumnAlign.Left, t => t.Name),
        new("Min", StringTableColumnAlign.Right, t => t.Min.ToString()),
        new("Active", StringTableColumnAlign.Right, t => t.Active.ToString()),
        new("Available", StringTableColumnAlign.Right, t => t.Available.ToString()),
        new("Max", StringTableColumnAlign.Right, t => t.Max.ToString())
    ];

    public static string ThreadPoolToStringTableString()
    {
        ThreadPool.GetMinThreads(out int minWorkerThreads, out int minCompletionPortThreads);
        ThreadPool.GetMaxThreads(out int maxWorkerThreads, out int maxCompletionPortThreads);
        ThreadPool.GetAvailableThreads(out int availableWorkerThreads, out int availableCompletionPortThreads);

        ThreadPoolRow[] threadPoolRows = new[]
        {
            new ThreadPoolRow("WorkerThreads", minWorkerThreads, maxWorkerThreads - availableWorkerThreads, availableWorkerThreads, maxWorkerThreads),
            new ThreadPoolRow("CompletionPortThreads", minCompletionPortThreads, maxCompletionPortThreads - availableCompletionPortThreads,
                availableCompletionPortThreads,
                maxCompletionPortThreads)
        };

        return threadPoolRows.ToString(ThreadPoolColumns);
    }

    private sealed class ThreadPoolRow(string name, int min, int active, int available, int max)
    {
        public readonly string Name = name;
        public readonly int Min = min;
        public readonly int Active = active;
        public readonly int Available = available;
        public readonly int Max = max;
    }
}