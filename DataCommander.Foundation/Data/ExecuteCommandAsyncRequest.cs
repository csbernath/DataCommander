namespace DataCommander.Foundation.Data
{
    using System;
    using System.Data.Common;
    using System.Threading.Tasks;
    using DataCommander.Foundation.Diagnostics.Contracts;

    public sealed class ExecuteCommandAsyncRequest
    {
        public ExecuteCommandAsyncRequest(CreateCommandRequest initializeCommandRequest, Func<DbCommand, Task> execute)
        {
            FoundationContract.Requires<ArgumentNullException>(initializeCommandRequest != null);
            FoundationContract.Requires<ArgumentNullException>(execute != null);

            InitializeCommandRequest = initializeCommandRequest;
            Execute = execute;
        }

        public readonly CreateCommandRequest InitializeCommandRequest;
        public readonly Func<DbCommand, Task> Execute;
    }
}