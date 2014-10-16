namespace DataCommander.Foundation.Threading.Tasks
{
#if FOUNDATION_3_5
#else
    using System.Threading.Tasks;

#endif

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    public sealed class CreateTaskResponse<TResult>
    {
        /// <summary>
        /// 
        /// </summary>
        public Task<TResult> Task;

        /// <summary>
        /// 
        /// </summary>
        public TaskInfo TaskInfo;
    }
}