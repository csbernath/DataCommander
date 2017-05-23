using System;
using System.Data;
using DataCommander.Foundation.Diagnostics.Contracts;

namespace DataCommander.Foundation.Data
{
    public sealed class ExecuteCommandRequest
    {
        public ExecuteCommandRequest(CreateCommandRequest createCommandRequest, Action<IDbCommand> execute)
        {
            FoundationContract.Requires<ArgumentNullException>(createCommandRequest != null);
            FoundationContract.Requires<ArgumentNullException>(execute != null);

            CreateCommandRequest = createCommandRequest;
            Execute = execute;
        }

        public readonly CreateCommandRequest CreateCommandRequest;
        public readonly Action<IDbCommand> Execute;
    }
}