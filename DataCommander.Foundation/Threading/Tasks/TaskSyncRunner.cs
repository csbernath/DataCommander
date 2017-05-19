namespace DataCommander.Foundation.Threading.Tasks
{
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// 
    /// </summary>
    public static class TaskSyncRunner
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="task"></param>
        /// <typeparam name="TResult"></typeparam>
        /// <returns></returns>
        public static TResult Run<TResult>(Func<Task<TResult>> task)
        {
            return Task.Run(async () => await task()).GetAwaiter().GetResult();
        }
    }
}