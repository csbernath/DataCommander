using System;
using System.Data.Common;
using System.Threading.Tasks;

namespace Foundation.Data;

public sealed class ExecuteCommandAsyncRequest
{
    public readonly CreateCommandRequest CreateCommandRequest;
    public readonly Func<DbCommand, Task> Execute;

    public ExecuteCommandAsyncRequest(CreateCommandRequest createCommandRequest, Func<DbCommand, Task> execute)
    {
        ArgumentNullException.ThrowIfNull(createCommandRequest);
        ArgumentNullException.ThrowIfNull(execute);

        CreateCommandRequest = createCommandRequest;
        Execute = execute;
    }
}