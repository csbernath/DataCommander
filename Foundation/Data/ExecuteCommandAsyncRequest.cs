using System;
using System.Data.Common;
using System.Threading.Tasks;
using Foundation.Diagnostics.Contracts;

namespace Foundation.Data
{
    public sealed class ExecuteCommandAsyncRequest
    {
        public ExecuteCommandAsyncRequest(CreateCommandRequest createCommandRequest, Func<DbCommand, Task> execute)
        {
            FoundationContract.Requires<ArgumentNullException>(createCommandRequest != null);
            FoundationContract.Requires<ArgumentNullException>(execute != null);

            CreateCommandRequest = createCommandRequest;
            Execute = execute;
        }

        public readonly CreateCommandRequest CreateCommandRequest;
        public readonly Func<DbCommand, Task> Execute;
    }
}