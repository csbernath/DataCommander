using System;
using System.Data.Common;
using System.Threading.Tasks;
using Foundation.Diagnostics.Assertions;

namespace Foundation.Data
{
    public sealed class ExecuteCommandAsyncRequest
    {
        public readonly CreateCommandRequest CreateCommandRequest;
        public readonly Func<DbCommand, Task> Execute;

        public ExecuteCommandAsyncRequest(CreateCommandRequest createCommandRequest, Func<DbCommand, Task> execute)
        {
            Assert.IsNotNull(createCommandRequest);
            Assert.IsNotNull(execute);

            CreateCommandRequest = createCommandRequest;
            Execute = execute;
        }
    }
}