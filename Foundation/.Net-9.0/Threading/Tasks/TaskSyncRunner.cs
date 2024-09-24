using System;
using System.Threading.Tasks;

namespace Foundation.Threading.Tasks;

public static class TaskSyncRunner
{
    public static TResult Run<TResult>(Func<Task<TResult>> task) => Task.Run(async () => await task()).GetAwaiter().GetResult();
}