using System;
using System.Data;
using Foundation.Diagnostics.Assertions;

namespace Foundation.Data
{
    public sealed class ExecuteCommandRequest
    {
        public readonly CreateCommandRequest CreateCommandRequest;
        public readonly Action<IDbCommand> Execute;

        public ExecuteCommandRequest(CreateCommandRequest createCommandRequest, Action<IDbCommand> execute)
        {
            Assert.IsNotNull(createCommandRequest);
            Assert.IsNotNull(execute);

            CreateCommandRequest = createCommandRequest;
            Execute = execute;
        }
    }
}