namespace DataCommander.Foundation.Orm
{
    using System;
    using System.Data.Common;
    using System.Threading;
    using System.Threading.Tasks;
    using Diagnostics.Contracts;

    /// <summary>
    /// 
    /// </summary>
    public sealed class ExecuteCommandRequest
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="createCommandRequest"></param>
        /// <param name="execute"></param>
        public ExecuteCommandRequest(CreateCommandRequest createCommandRequest, Func<DbCommand, Task> execute)
        {
            FoundationContract.Requires<ArgumentNullException>(createCommandRequest != null);
            FoundationContract.Requires<ArgumentNullException>(execute != null);

            CreateCommandRequest = createCommandRequest;
            Execute = execute;
        }

        /// <summary>
        /// 
        /// </summary>
        public readonly CreateCommandRequest CreateCommandRequest;

        /// <summary>
        /// 
        /// </summary>
        public readonly Func<DbCommand, Task> Execute;
    }
}