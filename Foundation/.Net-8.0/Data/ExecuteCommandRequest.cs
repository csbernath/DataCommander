using System;
using System.Data;

namespace Foundation.Data;

public sealed class ExecuteCommandRequest
{
    public readonly CreateCommandRequest CreateCommandRequest;
    public readonly Action<IDbCommand> Execute;

    public ExecuteCommandRequest(CreateCommandRequest createCommandRequest, Action<IDbCommand> execute)
    {
        ArgumentNullException.ThrowIfNull(createCommandRequest);
        ArgumentNullException.ThrowIfNull(execute);

        CreateCommandRequest = createCommandRequest;
        Execute = execute;
    }
}